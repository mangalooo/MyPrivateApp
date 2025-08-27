
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using System.Globalization;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesIndexYearsClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesIndexYearsClass> logger) : ISharesIndexYearsClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesIndexYearsClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> CalculateLastYearsResults()
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("CalculateLastYearsResults: db == null!");

                // Ensure there is at least one row to infer the next year from
                if (!await db.SharesProfitOrLossYears.AsNoTracking().AnyAsync())
                    return "Finns ingen rad i tabellen: SharesProfitOrLossYears. Måste finnas minst en rad!";

                // Determine which year to calculate next
                var yearStrings = await db.SharesProfitOrLossYears
                    .AsNoTracking()
                    .Where(y => !string.IsNullOrWhiteSpace(y.Year))
                    .Select(y => y.Year!)
                    .ToListAsync();

                int biggerYear = 0;
                foreach (var ys in yearStrings)
                {
                    if (int.TryParse(ys, out int y) && y > biggerYear)
                        biggerYear = y;
                    else
                        return "År saknas eller är ogiltigt i tabellen: SharesProfitOrLossYears. Måste fyllas i!";
                }

                int thisCalculationYear = biggerYear + 1;
                int thisYear = 2009; //DateTime.Now.Year; // Magnus: Ändra tillbaka när alla år är uppdaterat annars justera här.
                string thisYearsErrorMessage = "Man kan inte beräkna detta året än, man måste vänta tills efter nyår!";

                // Do not calculate for the current year or future
                if (thisCalculationYear >= thisYear)
                    return thisYearsErrorMessage;

                // Prevent duplicate calculation for the same year
                if (await db.SharesProfitOrLossYears.AsNoTracking().AnyAsync(x => x.Year == thisCalculationYear.ToString()))
                    return $"Året {thisCalculationYear} är redan beräknat.";

                // Load related data once to avoid multiple queries (tracked for flag updates)
                List<SharesSolds> sharesSoldsList = await db.SharesSolds.ToListAsync();
                List<SharesSoldFunds> sharesSoldFundsList = await db.SharesSoldFunds.ToListAsync();
                List<SharesDividend> sharesDividendsList = await db.SharesDividends.ToListAsync();
                List<SharesInterestRates> sharesInterestRatesList = await db.SharesInterestRates.ToListAsync();
                List<SharesFee> sharesFeesList = await db.SharesFees.ToListAsync();

                double sharesSolds = 0, fundsSold = 0;
                double fees = 0, taxes = 0, brokerage = 0;

                // Calculations (also toggle CalculationFlag on matched rows)
                double sharesPurchaseds = CalculateShares(sharesSoldsList, thisYear, thisCalculationYear, thisYearsErrorMessage, ref sharesSolds);
                double fundsPurchased = CalculateFunds(sharesSoldFundsList, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fundsSold);
                double dividends = CalculateDividends(sharesDividendsList, thisYear, thisCalculationYear, thisYearsErrorMessage);
                double interestRates = CalculateInterestRates(sharesInterestRatesList, thisYear, thisCalculationYear, thisYearsErrorMessage);
                CalculateFees(sharesFeesList, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fees, ref taxes, ref brokerage);

                // If nothing to aggregate, avoid creating an empty year entry
                bool hasAnyMovement =
                    sharesSolds != 0 || sharesPurchaseds != 0 ||
                    fundsSold != 0 || fundsPurchased != 0 ||
                    dividends != 0 || interestRates != 0 ||
                    fees != 0 || taxes != 0 || brokerage != 0;

                if (!hasAnyMovement)
                    return $"Inga transaktioner hittades för {thisCalculationYear}.";

                // Aggregations
                double sharesYearResult = sharesSolds - sharesPurchaseds;
                double fundsYearResult = fundsSold - fundsPurchased;
                double moneyProfitOrLossYear = (sharesYearResult + fundsYearResult + dividends + interestRates) - (fees + taxes + brokerage);
                double soldCalculation = (sharesSolds + fundsSold + dividends + interestRates) - (fees + taxes + brokerage);
                double purchasedSharesAndFunds = sharesPurchaseds + fundsPurchased;
                double percentProfitOrLossYear = purchasedSharesAndFunds > 0 ? (soldCalculation / purchasedSharesAndFunds) - 1 : 0;

                SharesProfitOrLossYears model = new()
                {
                    Year = thisCalculationYear.ToString(),
                    SharesYear = Math.Round(sharesYearResult, 2, MidpointRounding.AwayFromZero),
                    FundsYear = Math.Round(fundsYearResult, 2, MidpointRounding.AwayFromZero),
                    DividendYear = Math.Round(dividends, 2, MidpointRounding.AwayFromZero),
                    InterestRatesYear = Math.Round(interestRates, 2, MidpointRounding.AwayFromZero),
                    FeeYear = Math.Round(fees, 2, MidpointRounding.AwayFromZero),
                    TaxYear = Math.Round(taxes, 2, MidpointRounding.AwayFromZero),
                    BrokerageYear = Math.Round(brokerage, 2, MidpointRounding.AwayFromZero),
                    MoneyProfitOrLossYear = Math.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero),
                    PercentProfitOrLossYear = ConvertToPercentage(percentProfitOrLossYear),
                    Note =
                        $"Sålda aktier: {sharesSolds:#,##0.00} - Köpta aktier: {sharesPurchaseds:#,##0.00} = {(sharesSolds - sharesPurchaseds):#,##0.00}" +
                        $"\r\nSålda fonder: {fundsSold:#,##0.00} - Köpta fonder: {fundsPurchased:#,##0.00} = {(fundsSold - fundsPurchased):#,##0.00}" +
                        $"\r\nUtdelning: {dividends:#,##0.00}" +
                        $"\r\nSkatter: {taxes:#,##0.00} + Avgifter: {fees:#,##0.00} + Courtage: {brokerage:#,##0.00} = {Math.Round(taxes + fees + brokerage, 2, MidpointRounding.AwayFromZero):#,##0.00}"
                };

                await using var tx = await db.Database.BeginTransactionAsync();

                await db.SharesProfitOrLossYears.AddAsync(model);

                SharesTotalProfitsOrLosses? sharesTotalProfitsOrLosses = await db.SharesTotalProfitsOrLosses.FirstOrDefaultAsync();
                if (sharesTotalProfitsOrLosses != null)
                    sharesTotalProfitsOrLosses.TotalProfitOrLoss += Math.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero);

                await db.SaveChangesAsync();
                await tx.CommitAsync();

                return string.Empty;
            }
            catch (InvalidOperationException vex)
            {
                _logger.LogWarning(vex, "Beräkning av årsresultat avbröts.");
                return $"Beräkning av årsresultat avbröts. Felmedelande: {vex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lägg till årligt resultat misslyckades.");
                return $"Lägg till årligt resultat. Felmeddelande: {ex.Message}";
            }
        }

        private static double CalculateShares(List<SharesSolds> sharesSolds, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double sharesSoldsTotal)
        {
            double sharesPurchaseds = 0;
            foreach (var item in sharesSolds)
            {
                if (!TryGetYear(item.DateOfSold, out int checkYear))
                    throw new InvalidOperationException("Ogiltigt datum i SharesSolds.");

                if (checkYear == thisYear) throw new InvalidOperationException("Aktier: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    sharesPurchaseds += item.Amount;
                    sharesSoldsTotal += item.AmountSold;
                    item.CalculationFlag = true;
                }
            }
            return sharesPurchaseds;
        }

        private static double CalculateFunds(List<SharesSoldFunds> sharesSoldFunds, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double fundsSoldTotal)
        {
            double fundsPurchased = 0;
            foreach (var item in sharesSoldFunds)
            {
                if (!TryGetYear(item.DateOfSold, out int checkYear))
                    throw new InvalidOperationException("Datum för sålda fonder saknas eller är ogiltigt i tabellen: SharesSoldFunds.");

                if (checkYear == thisYear) throw new InvalidOperationException("Fonder: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    fundsPurchased += item.Amount;
                    fundsSoldTotal += item.AmountSold;
                    item.CalculationFlag = true;
                }
            }
            return fundsPurchased;
        }

        private static double CalculateDividends(List<SharesDividend> sharesDividends, int thisYear, int thisCalculationYear, string thisYearsErrorMessage)
        {
            double dividends = 0;
            foreach (var item in sharesDividends)
            {
                if (!TryGetYear(item.Date, out int checkYear))
                    throw new InvalidOperationException("Datum för utdelning saknas eller är ogiltigt i tabellen: SharesDividend.");

                if (checkYear == thisYear) throw new InvalidOperationException("Utdelaning: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    dividends += item.TotalAmount;
                    item.CalculationFlag = true;
                }
            }
            return dividends;
        }

        private static double CalculateInterestRates(List<SharesInterestRates> sharesInterestRates, int thisYear, int thisCalculationYear, string thisYearsErrorMessage)
        {
            double interestRates = 0;
            foreach (var item in sharesInterestRates)
            {
                if (!TryGetYear(item.Date, out int checkYear))
                    throw new InvalidOperationException("Datum för ränta saknas eller är ogiltigt i tabellen: SharesInterestRates.");

                if (checkYear == thisYear) throw new InvalidOperationException("Ränta: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    interestRates += item.TotalAmount;
                    item.CalculationFlag = true;
                }
            }
            return interestRates;
        }

        private static void CalculateFees(List<SharesFee> sharesFees, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double fees, ref double taxes, ref double brokerage)
        {
            foreach (var item in sharesFees)
            {
                if (!TryGetYear(item.Date, out int checkYear))
                    throw new InvalidOperationException("Datum för avgift saknas eller är ogiltigt i tabellen: SharesFee.");

                if (checkYear == thisYear) throw new InvalidOperationException("Kostnader: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    fees += item.Fee;
                    taxes += item.Tax;
                    brokerage += item.Brokerage;
                    item.CalculationFlag = true;
                }
            }
        }

        public async Task<string> Delete(SharesProfitOrLossYears model)
        {
            await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

            if (model == null || model.SharesProfitOrLossYearsId <= 0)
                return "Aktien saknar data i formuläret!";

            try
            {
                // Remove the year entry
                db.SharesProfitOrLossYears.Remove(model);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ta bort årligt resultat misslyckades.");
                return $"Ta bort. Felmeddelande: {ex.Message}";
            }
        }

        public SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model)
        {
            return new SharesProfitOrLossYearViewModel
            {
                SharesProfitOrLossYearsId = model.SharesProfitOrLossYearsId,
                Year = model.Year,
                SharesYear = model.SharesYear.ToString("#,##0.00"),
                FundsYear = model.FundsYear.ToString("#,##0.00"),
                DividendYear = model.DividendYear.ToString("#,##0.00"),
                InterestRatesYear = model.InterestRatesYear.ToString("#,##0.00"),
                FeeYear = model.FeeYear.ToString("#,##0.00"),
                TaxYear = model.TaxYear.ToString("#,##0.00"),
                BrokerageYear = model.BrokerageYear.ToString("#,##0.00"),
                MoneyProfitOrLossYear = model.MoneyProfitOrLossYear.ToString("#,##0.00"),
                PercentProfitOrLossYear = model.PercentProfitOrLossYear,
                Note = model.Note
            };
        }

        public async Task<SharesTotalProfitsOrLosses> GetTotalProfitsOrLosses(int? id)
        {
            if (id <= 0)
                throw new Exception("Get: Finns inget ID!");

            await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("GetTotalProfitsOrLosses: db == null!");

            return await db.SharesTotalProfitsOrLosses.FirstOrDefaultAsync(r => r.SharesTotalProfitOrLossId == id)
                   ?? throw new Exception("Totala summan hittades inte i databasen!");
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private static bool TryGetYear(string? dateString, out int year)
        {
            year = 0;
            if (string.IsNullOrWhiteSpace(dateString)) return false;

            // Try multiple cultures and common formats
            var cultures = new[] { CultureInfo.CurrentCulture, CultureInfo.GetCultureInfo("sv-SE"), CultureInfo.InvariantCulture };
            foreach (var c in cultures)
            {
                if (DateTime.TryParse(dateString, c, DateTimeStyles.AssumeLocal, out var dt))
                {
                    year = dt.Year;
                    return true;
                }
            }

            // Common ISO formats as a fallback
            string[] formats = ["yyyy-MM-dd", "yyyyMMdd", "dd/MM/yyyy", "dd-MM-yyyy"];
            foreach (var c in cultures)
            {
                if (DateTime.TryParseExact(dateString, formats, c, DateTimeStyles.AssumeLocal, out var dt))
                {
                    year = dt.Year;
                    return true;
                }
            }

            return false;
        }
    }
}


// Magnus: Testa GPT-5 version av denna funktion, funkar de så ta bort denna

//public async Task<string> CalculateLastYearsResults()
//{
//    try
//    {
//        await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("CalculateLastYearsResults: db == null!");

//        if (!db.SharesProfitOrLossYears.Any())
//            return "SharesProfitOrLossYears databasen är tom!";

//        int thisYear = 2009; //DateTime.Now.Year; // Magnus: Ändra tillbaka när alla år är uppdaterat annars justera här.
//        double sharesPurchaseds, sharesSolds = 0, fundsPurchased, fundsSold = 0;
//        double dividends, interestRates, fees = 0, taxes = 0, brokerage = 0;
//        string thisYearsErrorMessage = "Man kan inte beräknad detta året än, man måste vänta tills efter nyår!";
//        int thisCalculationYear = 0, biggerYear = 0;

//        if (!db.SharesProfitOrLossYears.Any())
//            return "Finns inget rad i tabellen: SharesProfitOrLossYears. Måste finns minst en rad!";

//        foreach (var item in db.SharesProfitOrLossYears)
//        {
//            if (string.IsNullOrEmpty(item.Year))
//                return "År saknas i tabellen: SharesProfitOrLossYears. Måste fyllas i!";

//            int itemYear = int.Parse(item.Year);
//            if (itemYear > biggerYear)
//            {
//                thisCalculationYear = itemYear + 1;
//                biggerYear = itemYear;
//            }
//        }

//        sharesPurchaseds = CalculateShares(db.SharesSolds, thisYear, thisCalculationYear, thisYearsErrorMessage, ref sharesSolds);
//        fundsPurchased = CalculateFunds(db.SharesSoldFunds, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fundsSold);
//        dividends = CalculateDividends(db.SharesDividends, thisYear, thisCalculationYear, thisYearsErrorMessage);
//        interestRates = CalculateInterestRates(db.SharesInterestRates, thisYear, thisCalculationYear, thisYearsErrorMessage);
//        CalculateFees(db.SharesFees, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fees, ref taxes, ref brokerage);

//        double sharesYearResult = sharesSolds - sharesPurchaseds;
//        double fundsYearResult = fundsSold - fundsPurchased;
//        double moneyProfitOrLossYear = (sharesYearResult + fundsYearResult + dividends + interestRates) - (fees + taxes + brokerage);
//        double soldCalculation = (sharesSolds + fundsSold + dividends + interestRates) - (fees + taxes + brokerage);
//        double purchasedSharesAndFunds = sharesPurchaseds + fundsPurchased;
//        double percentProfitOrLossYear = (soldCalculation / purchasedSharesAndFunds) - 1;

//        SharesProfitOrLossYears model = new()
//        {
//            Year = thisCalculationYear.ToString(),
//            SharesYear = double.Round(sharesYearResult, 2, MidpointRounding.AwayFromZero),
//            FundsYear = double.Round(fundsYearResult, 2, MidpointRounding.AwayFromZero),
//            DividendYear = double.Round(dividends, 2, MidpointRounding.AwayFromZero),
//            InterestRatesYear = double.Round(interestRates, 2, MidpointRounding.AwayFromZero),
//            FeeYear = double.Round(fees, 2, MidpointRounding.AwayFromZero),
//            TaxYear = double.Round(taxes, 2, MidpointRounding.AwayFromZero),
//            BrokerageYear = double.Round(brokerage, 2, MidpointRounding.AwayFromZero),
//            MoneyProfitOrLossYear = double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero),
//            PercentProfitOrLossYear = ConvertToPercentage(percentProfitOrLossYear),
//            Note = $"Sålda Aktier: {sharesSolds} - Köpta aktier: {sharesPurchaseds} = {sharesSolds - sharesPurchaseds}" +
//                   $"\r\nSålda fonder: {fundsSold} - Köpta fonder: {fundsPurchased} = {fundsSold - fundsPurchased}" +
//                   $"\r\nUtdelning: {dividends}" +
//                   $"\r\nSkatter: {taxes} + Avgifter: {fees} + Courtage: {brokerage} = {double.Round(taxes + fees + brokerage, 2, MidpointRounding.AwayFromZero)}"
//        };

//        await db.SharesProfitOrLossYears.AddAsync(model);
//        SharesTotalProfitsOrLosses? sharesTotalProfitsOrLosses = await db.SharesTotalProfitsOrLosses.FirstOrDefaultAsync();

//        if (sharesTotalProfitsOrLosses != null)
//            sharesTotalProfitsOrLosses.TotalProfitOrLoss += double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero);

//        await db.SaveChangesAsync();
//    }
//    catch (Exception ex)
//    {
//        return $"Lägg till årligt resultat. Felmeddelande: {ex.Message}";
//    }

//    return string.Empty;
//}