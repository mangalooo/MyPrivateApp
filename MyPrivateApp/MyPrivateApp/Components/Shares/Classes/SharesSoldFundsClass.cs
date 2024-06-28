using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldFundsClass : ISharesSoldFundsClass
    {
        private static SharesSoldFunds? Get(ApplicationDbContext db, string ISIN) => db.SharesSoldFunds.Any(r => r.ISIN == ISIN) ?
                                                                                    db.SharesSoldFunds.FirstOrDefault(r => r.ISIN == ISIN) :
                                                                                        throw new Exception("Den köpa fonden hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesSoldFundViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.FundName) && !string.IsNullOrEmpty(vm.ISIN) &&
                        vm.HowMany > 0 && vm.PricePerFunds > 0 && vm.Fee > 0)
                {
                    SharesSoldFunds model = ChangeFromViewModelToModel(vm);

                    try
                    {
                        db.SharesSoldFunds.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Lägg till", import, ex.Message);
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Lägg till", import, "Du måste fylla i fälten: Fond namn, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!");
                    else
                        return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Lägg till", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            }

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, SharesSoldFundViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.FundName) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.HowMany > 0 && vm.PricePerFunds > 0 && vm.Fee > 0 && vm.DateOfSold != DateTime.MinValue && vm.PricePerFundsSold > 0)
                {
                    try
                    {
                        SharesSoldFunds? dbModel = Get(db, vm.ISIN);

                        if (dbModel != null)
                        {
                            SharesSoldFunds model = ChangeFromViewModelToModel(vm);

                            dbModel.DateOfPurchase = model.DateOfPurchase;
                            dbModel.Amount = double.Round(model.HowMany * model.PricePerFunds, 2, MidpointRounding.AwayFromZero);
                            dbModel.DateOfSold = model.DateOfSold;
                            dbModel.AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero);
                            dbModel.FundName = model.FundName;
                            dbModel.HowMany = model.HowMany;
                            dbModel.PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero);
                            dbModel.PricePerFundsSold = double.Round(model.PricePerFundsSold, 2, MidpointRounding.AwayFromZero);
                            dbModel.Currency = model.Currency;
                            dbModel.ISIN = model.ISIN;
                            dbModel.Account = model.Account;
                            dbModel.Fee = model.Fee;
                            dbModel.TypeOfFund = model.TypeOfFund;
                            dbModel.MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero);
                            dbModel.PercentProfitOrLoss = model.PercentProfitOrLoss;
                            dbModel.Note = model.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Fonden hittades inte i databasen!!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", import, ex.Message);
                    }
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per fond del, pris per såld fond, Säljdatum och Avgift!";
            }
            else
                return "Fonden hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, SharesSoldFundViewModel vm, bool import)
        {
            if (vm != null && vm.AmountSold > 0 && db != null)
            {
                SharesSoldFunds model = ChangeFromViewModelToModel(vm);

                try
                {
                    db.ChangeTracker.Clear();
                    db.SharesSoldFunds.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
                }
            }
            else
                return "Fonden hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public SharesSoldFundViewModel ChangeFromModelToViewModel(SharesSoldFunds model)
        {
            DateTime dateOfPurchase = DateTime.Parse(model.DateOfPurchase);
            DateTime dateOfSold = DateTime.Parse(model.DateOfSold);

            SharesSoldFundViewModel vm = new()
            {
                SharesSoldFundId = model.SharesSoldFundId,
                DateOfPurchase = dateOfPurchase,
                DateOfSold = dateOfSold,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero),
                FundName = model.FundName,
                HowMany = model.HowMany,
                PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                PricePerFundsSold = double.Round(model.PricePerFundsSold, 2, MidpointRounding.AwayFromZero),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Fee = model.Fee,
                TypeOfFund = model.TypeOfFund,
                MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero),
                PercentProfitOrLoss = model.PercentProfitOrLoss,
                Note = model.Note
            };

            return vm;
        }

        private static SharesSoldFunds ChangeFromViewModelToModel(SharesSoldFundViewModel vm)
        {
            SharesSoldFunds sharesSoldFunds = new()
            {
                SharesSoldFundId = vm.SharesSoldFundId,
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd"),
                Amount = double.Round(vm.HowMany * vm.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(vm.HowMany * vm.PricePerFundsSold, 2, MidpointRounding.AwayFromZero),
                FundName = vm.FundName,
                HowMany = vm.HowMany,
                PricePerFunds = double.Round(vm.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                PricePerFundsSold = double.Round(vm.PricePerFundsSold, 2, MidpointRounding.AwayFromZero),
                Fee = vm.Fee,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Account = vm.Account,
                TypeOfFund = vm.TypeOfFund,
                Note = vm.Note
            };

            sharesSoldFunds.MoneyProfitOrLoss = sharesSoldFunds.AmountSold - sharesSoldFunds.Amount;

            double calculateMoneyProfitOrLoss = (sharesSoldFunds.AmountSold / sharesSoldFunds.Amount) - 1;

            sharesSoldFunds.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

            return sharesSoldFunds;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private static void ErrorHandling(ApplicationDbContext db, SharesSoldFundViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} såld fond: Fond namn: {vm.FundName}, " +
                        $"Datum: {vm.DateOfPurchase}, Id: {vm.SharesSoldFundId}, ISIN: {vm.ISIN}."
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}