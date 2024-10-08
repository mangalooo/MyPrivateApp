using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesIndexYearsClass : ISharesIndexYearsClass
    {
        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        public string CalculateLastYearsResults(ApplicationDbContext db)
        {
            if (db == null) return "Ingen kontakt med databasen";
            if (db.SharesProfitOrLossYears.Count() == 0) return "Befintlig databas är tom!";

            int thisYear = DateTime.Now.Year;
            double sharesPurchaseds = 0;
            double sharesSolds = 0;
            double fundsPurchased = 0; ;
            double fundsSold = 0; ;
            double dividends = 0;
            double interestRates = 0;
            double fees = 0;
            double brokerage = 0;
            string thisYearsErrorMessage = "Man kan inte beräknad detta året än, man måste vänta tills efter nyår!";
            int thisCalculationYear = 0;
            int biggerYear = 0;

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

                    if (checkYear == thisYear) return "Skatt/avgifter: " + thisYearsErrorMessage;

                    if (checkYear == thisCalculationYear && item.CalculationFlag == false)
                    {
                        fees += item.Tax;
                        brokerage += item.Brokerage;
                        item.CalculationFlag = true;
                        db.SaveChanges();
                    }
                }
            }

            try
            {
                double sharesYear = sharesSolds - sharesPurchaseds;
                double fundsYear = fundsSold - fundsPurchased;
                double moneyProfitOrLossYear = (sharesYear + fundsYear + dividends + interestRates) - (fees + brokerage);
                double soldCalculation = (sharesSolds + fundsSold + dividends + interestRates) - (fees + brokerage);
                double purchasedSharesAndFunds = sharesPurchaseds + fundsPurchased;
                double percentProfitOrLossYear = (soldCalculation / purchasedSharesAndFunds) - 1;

                SharesProfitOrLossYears model = new()
                {
                    Year = thisCalculationYear.ToString(),
                    SharesYear = sharesYear,
                    FundsYear = fundsYear,
                    DividendYear = dividends,
                    InterestRatesYear = interestRates,
                    FeeYear = fees,
                    BrokerageYear = brokerage,
                    MoneyProfitOrLossYear = moneyProfitOrLossYear,
                    PercentProfitOrLossYear = ConvertToPercentage(percentProfitOrLossYear)
                };

                db.SharesProfitOrLossYears.Add(model);
                SharesTotalAmounts sharesTotalAmounts = db.SharesTotalAmounts.FirstOrDefault();
                sharesTotalAmounts.TotalAmount += moneyProfitOrLossYear;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return $"Lägg till. Felmeddelande: {ex.Message}";
            }

            return string.Empty;
        }

        public SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model)
        {
            SharesProfitOrLossYearViewModel vm = new()
            {
                SharesProfitOrLossYearsId = model.SharesProfitOrLossYearsId,
                Year = model.Year,
                SharesYear = model.SharesYear,
                FundsYear = model.FundsYear,
                DividendYear = model.DividendYear,
                InterestRatesYear = model.InterestRatesYear,
                FeeYear = model.FeeYear,
                BrokerageYear = model.BrokerageYear,
                MoneyProfitOrLossYear = model.MoneyProfitOrLossYear,
                PercentProfitOrLossYear = model.PercentProfitOrLossYear
            };

            return vm;
        }

        private static SharesProfitOrLossYears ChangeFromViewModelToModel(SharesProfitOrLossYearViewModel vm)
        {
            SharesProfitOrLossYears model = new()
            {
                SharesProfitOrLossYearsId = vm.SharesProfitOrLossYearsId,
                Year = vm.Year,
                SharesYear = vm.SharesYear,
                FundsYear = vm.FundsYear,
                DividendYear = vm.DividendYear,
                InterestRatesYear = vm.InterestRatesYear,
                FeeYear = vm.FeeYear,
                BrokerageYear = vm.BrokerageYear,
                MoneyProfitOrLossYear = vm.MoneyProfitOrLossYear,
                PercentProfitOrLossYear = vm.PercentProfitOrLossYear
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