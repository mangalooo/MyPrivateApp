using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesIndexYearsClass : ISharesIndexYearsClass
    {
        public SharesTotalProfitsOrLosses? GetTotalProfitsOrLosses(ApplicationDbContext db, int? id) => db.SharesTotalProfitsOrLosses.Any(r => r.SharesTotalProfitOrLossId == id) ?
                                                                                                 db.SharesTotalProfitsOrLosses.FirstOrDefault(r => r.SharesTotalProfitOrLossId == id) :
                                                                                                     throw new Exception("Totala summan hittades inte i databasen!");

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        public string CalculateLastYearsResults(ApplicationDbContext db)
        {
            if (db == null) return "Ingen kontakt med databasen";
            if (!db.SharesProfitOrLossYears.Any()) return "Befintlig databas är tom!";

            int thisYear = 2010; //DateTime.Now.Year; // Magnus: Ändra tillbaka
            double sharesPurchaseds = 0;
            double sharesSolds = 0;
            double fundsPurchased = 0; ;
            double fundsSold = 0; ;
            double dividends = 0;
            double interestRates = 0;
            double fees = 0;
            double taxes = 0;
            double brokerage = 0;
            string thisYearsErrorMessage = "Man kan inte beräknad detta året än, man måste vänta tills efter nyår!";
            int thisCalculationYear = 0;
            int biggerYear = 0;

            if (!db.SharesProfitOrLossYears.Any()) return "Finns inget i tabellen: SharesProfitOrLossYears. Måste finns en rad!";

            foreach (var item in db.SharesProfitOrLossYears)
            {
                int itemYear = int.Parse(item.Year);

                if (itemYear > biggerYear)
                {
                    thisCalculationYear = itemYear + 1;
                    biggerYear = itemYear;
                }
            }

            if (db.SharesSolds.Any())
            {
                foreach (var item in db.SharesSolds)
                {
                    int checkYear = DateTime.Parse(item.DateOfSold).Year;

                    if (checkYear == thisYear) return "Aktier: " + thisYearsErrorMessage;

                    if (checkYear == thisCalculationYear && item.CalculationFlag == false)
                    {
                        sharesPurchaseds += item.Amount;
                        sharesSolds += item.AmountSold;
                        item.CalculationFlag = true;
                        db.SaveChanges();
                    }
                }
            }

            if (db.SharesSoldFunds.Any())
            {
                foreach (var item in db.SharesSoldFunds)
                {
                    int checkYear = DateTime.Parse(item.DateOfSold).Year;

                    if (checkYear == thisYear) return "Fonder: " + thisYearsErrorMessage;

                    if (checkYear == thisCalculationYear && item.CalculationFlag == false)
                    {
                        fundsPurchased += item.Amount;
                        fundsSold += item.AmountSold;
                        item.CalculationFlag = true;
                        db.SaveChanges();
                    }
                }
            }

            if (db.SharesDividends.Any())
            {
                foreach (var item in db.SharesDividends)
                {
                    int checkYear = DateTime.Parse(item.Date).Year;

                    if (checkYear == thisYear) return "Utdelaning: " + thisYearsErrorMessage;

                    if (checkYear == thisCalculationYear && item.CalculationFlag == false)
                    {
                        dividends += item.TotalAmount;
                        item.CalculationFlag = true;
                        db.SaveChanges();
                    }
                }
            }

            if (db.SharesInterestRates.Any())
            {
                foreach (var item in db.SharesInterestRates)
                {
                    int checkYear = DateTime.Parse(item.Date).Year;

                    if (checkYear == thisYear) return "Ränta: " + thisYearsErrorMessage;

                    if (checkYear == thisCalculationYear && item.CalculationFlag == false)
                    {
                        interestRates += item.TotalAmount;
                        item.CalculationFlag = true;
                        db.SaveChanges();
                    } 
                }
            }

            if (db.SharesFees.Any())
            {
                foreach (var item in db.SharesFees)
                {
                    int checkYear = DateTime.Parse(item.Date).Year;

                    if (checkYear == thisYear) return "Kostnader: " + thisYearsErrorMessage;

                    if (checkYear == thisCalculationYear && item.CalculationFlag == false)
                    {
                        fees += item.Fee;
                        taxes += item.Tax;
                        brokerage += item.Brokerage;
                        item.CalculationFlag = true;
                        db.SaveChanges();
                    }
                }
            }

            try
            {
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
                    BrokerageYear = double.Round(brokerage, 2, MidpointRounding.AwayFromZero),
                    MoneyProfitOrLossYear = double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero),
                    PercentProfitOrLossYear = ConvertToPercentage(percentProfitOrLossYear),
                    Note = $"Sålda Aktier: {sharesSolds} - Köpta aktier: {sharesPurchaseds} = {sharesSolds - sharesPurchaseds}" +
                           $"\r\nSålda fonder: {fundsSold} - Köpta fonder: {fundsPurchased} = {fundsSold - fundsPurchased}" +
                           $"\r\nUtdelning: {dividends}" +
                           $"\r\nSkatt: {fees} + Courtage: {brokerage} = {double.Round(fees + brokerage, 2, MidpointRounding.AwayFromZero)}"
                };

                db.SharesProfitOrLossYears.Add(model);
                SharesTotalProfitsOrLosses sharesTotalProfitsOrLosses = db.SharesTotalProfitsOrLosses.FirstOrDefault();
                sharesTotalProfitsOrLosses.TotalProfitOrLoss += double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return $"Lägg till årligt resultat. Felmeddelande: {ex.Message}";
            }

            return string.Empty;
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
                BrokerageYear = model.BrokerageYear.ToString("#,##0.00"),
                MoneyProfitOrLossYear = model.MoneyProfitOrLossYear.ToString("#,##0.00"),
                PercentProfitOrLossYear = model.PercentProfitOrLossYear,
                Note = model.Note
            };

            return vm;
        }

        private static SharesProfitOrLossYears ChangeFromViewModelToModel(SharesProfitOrLossYearViewModel vm)
        {
            SharesProfitOrLossYears model = new()
            {
                SharesProfitOrLossYearsId = vm.SharesProfitOrLossYearsId,
                Year = vm.Year,
                SharesYear = double.Parse(vm.SharesYear),
                FundsYear = double.Parse(vm.FundsYear),
                DividendYear = double.Parse(vm.DividendYear),
                InterestRatesYear = double.Parse(vm.InterestRatesYear),
                FeeYear = double.Parse(vm.FeeYear),
                BrokerageYear = double.Parse(vm.BrokerageYear),
                MoneyProfitOrLossYear = double.Parse(vm.MoneyProfitOrLossYear),
                PercentProfitOrLossYear = vm.PercentProfitOrLossYear,
                Note = vm.Note
            };

            return model;
        }

        public string Delete(ApplicationDbContext db, SharesProfitOrLossYearViewModel vm)
        {
            SharesProfitOrLossYears model = ChangeFromViewModelToModel(vm);

            if (db != null && model != null)
            {
                try
                {
                    db.ChangeTracker.Clear();
                    db.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Ta bort. Felmeddelande: {ex.Message}";
                }

                db.SaveChanges();
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }
    }
}