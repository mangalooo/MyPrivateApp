
using AutoMapper;
using MyPrivateApp.Components.FarmWork.Classes;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesIndexYearsClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmWorkClass> logger) : ISharesIndexYearsClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmWorkClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<SharesTotalProfitsOrLosses> GetTotalProfitsOrLosses(int? id)
        {
            if (id <= 0)
                throw new Exception("Get: Finns inget ID!");

            await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("GetTotalProfitsOrLosses: db == null!");

            return await db.SharesTotalProfitsOrLosses.FirstOrDefaultAsync(r => r.SharesTotalProfitOrLossId == id)
                   ?? throw new Exception("Totala summan hittades inte i databasen!");
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        public async Task<string> CalculateLastYearsResults()
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("CalculateLastYearsResults: db == null!");

                if (!db.SharesProfitOrLossYears.Any())
                    return "Befintlig databas är tom!";

                int thisYear = 2012; //DateTime.Now.Year; // Magnus: Ändra tillbaka
                double sharesPurchaseds, sharesSolds = 0, fundsPurchased, fundsSold = 0;
                double dividends, interestRates, fees = 0, taxes = 0, brokerage = 0;
                string thisYearsErrorMessage = "Man kan inte beräknad detta året än, man måste vänta tills efter nyår!";
                int thisCalculationYear = 0, biggerYear = 0;

                if (!db.SharesProfitOrLossYears.Any())
                    return "Finns inget rad i tabellen: SharesProfitOrLossYears. Måste finns minst en rad!";

                foreach (var item in db.SharesProfitOrLossYears)
                {
                    if (string.IsNullOrEmpty(item.Year))
                        return "År saknas i tabellen: SharesProfitOrLossYears. Måste fyllas i!";

                    int itemYear = int.Parse(item.Year);
                    if (itemYear > biggerYear)
                    {
                        thisCalculationYear = itemYear + 1;
                        biggerYear = itemYear;
                    }
                }

                sharesPurchaseds = CalculateShares(db.SharesSolds, thisYear, thisCalculationYear, thisYearsErrorMessage, ref sharesSolds);
                fundsPurchased = CalculateFunds(db.SharesSoldFunds, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fundsSold);
                dividends = CalculateDividends(db.SharesDividends, thisYear, thisCalculationYear, thisYearsErrorMessage);
                interestRates = CalculateInterestRates(db.SharesInterestRates, thisYear, thisCalculationYear, thisYearsErrorMessage);
                CalculateFees(db.SharesFees, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fees, ref taxes, ref brokerage);

                double sharesYearResult = sharesSolds - sharesPurchaseds;
                double fundsYearResult = fundsSold - fundsPurchased;
                double moneyProfitOrLossYear = (sharesYearResult + fundsYearResult + dividends + interestRates) - (fees + taxes + brokerage);
                double soldCalculation = (sharesSolds + fundsSold + dividends + interestRates) - (fees + taxes + brokerage);
                double purchasedSharesAndFunds = sharesPurchaseds + fundsPurchased;
                double percentProfitOrLossYear = (soldCalculation / purchasedSharesAndFunds) - 1;

                SharesProfitOrLossYears model = new()
                {
                    Year = thisCalculationYear.ToString(),
                    SharesYear = double.Round(sharesYearResult, 2, MidpointRounding.AwayFromZero),
                    FundsYear = double.Round(fundsYearResult, 2, MidpointRounding.AwayFromZero),
                    DividendYear = double.Round(dividends, 2, MidpointRounding.AwayFromZero),
                    InterestRatesYear = double.Round(interestRates, 2, MidpointRounding.AwayFromZero),
                    FeeYear = double.Round(fees, 2, MidpointRounding.AwayFromZero),
                    TaxYear = double.Round(taxes, 2, MidpointRounding.AwayFromZero),
                    BrokerageYear = double.Round(brokerage, 2, MidpointRounding.AwayFromZero),
                    MoneyProfitOrLossYear = double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero),
                    PercentProfitOrLossYear = ConvertToPercentage(percentProfitOrLossYear),
                    Note = $"Sålda Aktier: {sharesSolds} - Köpta aktier: {sharesPurchaseds} = {sharesSolds - sharesPurchaseds}" +
                           $"\r\nSålda fonder: {fundsSold} - Köpta fonder: {fundsPurchased} = {fundsSold - fundsPurchased}" +
                           $"\r\nUtdelning: {dividends}" +
                           $"\r\nSkatter: {taxes} + Avgifter: {fees} + Courtage: {brokerage} = {double.Round(taxes + fees + brokerage, 2, MidpointRounding.AwayFromZero)}"
                };

                await db.SharesProfitOrLossYears.AddAsync(model);
                SharesTotalProfitsOrLosses? sharesTotalProfitsOrLosses = await db.SharesTotalProfitsOrLosses.FirstOrDefaultAsync();

                if (sharesTotalProfitsOrLosses != null)
                    sharesTotalProfitsOrLosses.TotalProfitOrLoss += double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return $"Lägg till årligt resultat. Felmeddelande: {ex.Message}";
            }

            return string.Empty;
        }

        private static double CalculateShares(IEnumerable<SharesSolds> sharesSolds, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double sharesSoldsTotal)
        {
            double sharesPurchaseds = 0;
            foreach (var item in sharesSolds)
            {
                int checkYear = DateTime.Parse(item.DateOfSold).Year;
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

        private static double CalculateFunds(IEnumerable<SharesSoldFunds> sharesSoldFunds, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double fundsSoldTotal)
        {
            double fundsPurchased = 0;
            foreach (var item in sharesSoldFunds)
            {
                if (item.DateOfSold == null)
                    throw new InvalidOperationException("Datum för sålda fonder saknas i tabellen: SharesSoldFunds.");

                int checkYear = DateTime.Parse(item.DateOfSold).Year;
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

        private static double CalculateDividends(IEnumerable<SharesDividend> sharesDividends, int thisYear, int thisCalculationYear, string thisYearsErrorMessage)
        {
            double dividends = 0;
            foreach (var item in sharesDividends)
            {
                if (item.Date == null)
                    throw new InvalidOperationException("Datum för utdelning saknas i tabellen: SharesDividend.");

                int checkYear = DateTime.Parse(item.Date).Year;
                if (checkYear == thisYear) throw new InvalidOperationException("Utdelaning: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    dividends += item.TotalAmount;
                    item.CalculationFlag = true;
                }
            }
            return dividends;
        }

        private static double CalculateInterestRates(IEnumerable<SharesInterestRates> sharesInterestRates, int thisYear, int thisCalculationYear, string thisYearsErrorMessage)
        {
            double interestRates = 0;
            foreach (var item in sharesInterestRates)
            {
                if (item.Date == null)
                    throw new InvalidOperationException("Datum för ränta saknas i tabellen: SharesInterestRates.");

                int checkYear = DateTime.Parse(item.Date).Year;
                if (checkYear == thisYear) throw new InvalidOperationException("Ränta: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    interestRates += item.TotalAmount;
                    item.CalculationFlag = true;
                }
            }
            return interestRates;
        }

        private static void CalculateFees(IEnumerable<SharesFee> sharesFees, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double fees, ref double taxes, ref double brokerage)
        {
            foreach (var item in sharesFees)
            {
                if (item.Date == null)
                    throw new InvalidOperationException("Datum för ränta saknas i tabellen: SharesFee.");

                int checkYear = DateTime.Parse(item.Date).Year;
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
                db.ChangeTracker.Clear();
                db.SharesProfitOrLossYears.Remove(model);
                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Ta bort. Felmeddelande: {ex.Message}";
            }
        }

        public SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model)
        {
            SharesProfitOrLossYearViewModel vm = new()
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

            return vm;
        }
    }
}