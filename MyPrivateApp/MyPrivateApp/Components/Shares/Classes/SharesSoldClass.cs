using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldClass : ISharesSoldClass
    {
        private static SharesSolds? Get(ApplicationDbContext db, int id) => db.SharesSolds.Any(r => r.SharesSoldId == id) ?
                                                                                    db.SharesSolds.FirstOrDefault(r => r.SharesSoldId == id) :
                                                                                        throw new Exception("Den sålda aktien hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) && !string.IsNullOrEmpty(vm.ISIN) &&
                        vm.HowMany > 0 && string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage > 0)
                {
                    SharesSolds model = ChangeFromViewModelToModel(vm);

                    try
                    {
                        db.SharesSolds.Add(model);
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
                        ErrorHandling(db, vm, "Lägg till", import, "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!");
                    else
                        return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!";
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

        public string Edit(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.HowMany > 0 && string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage > 0 && vm.DateOfSold != DateTime.MinValue && string.IsNullOrEmpty(vm.PricePerSharesSold))
                {
                    try
                    {
                        SharesSolds? dbModel = Get(db, vm.SharesSoldId);

                        if (dbModel != null)
                        {
                            SharesSolds model = ChangeFromViewModelToModel(vm);

                            dbModel.DateOfPurchase = model.DateOfPurchase;
                            dbModel.Amount = double.Round(model.HowMany * model.PricePerShares, 2, MidpointRounding.AwayFromZero);
                            dbModel.DateOfSold = model.DateOfSold;
                            dbModel.AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero);
                            dbModel.CompanyName = model.CompanyName;
                            dbModel.HowMany = model.HowMany;
                            dbModel.PricePerShares = double.Round(model.PricePerShares, 2, MidpointRounding.AwayFromZero);
                            dbModel.PricePerSharesSold = double.Round(model.PricePerSharesSold, 2, MidpointRounding.AwayFromZero);
                            dbModel.Currency = model.Currency;
                            dbModel.ISIN = model.ISIN;
                            dbModel.Account = model.Account;
                            dbModel.Brokerage = model.Brokerage;
                            dbModel.TypeOfShares = model.TypeOfShares;
                            dbModel.MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero);
                            dbModel.PercentProfitOrLoss = model.PercentProfitOrLoss;
                            dbModel.Note = model.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Aktien hittades inte i databasen!!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", import, ex.Message);
                    }
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, pris per såld aktie, Säljdatum och Courage!";
            }
            else
                return "Aktien hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            if (vm != null && vm.SharesSoldId > 0 && db != null)
            {
                SharesSolds model = ChangeFromViewModelToModel(vm);

                try
                {
                    db.ChangeTracker.Clear();
                    db.SharesSolds.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
                }
            }
            else
                return "Aktien hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model)
        {
            DateTime dateOfPurchase = DateTime.Parse(model.DateOfPurchase);
            DateTime dateOfSold = DateTime.Parse(model.DateOfSold);

            SharesSoldViewModel vm = new()
            {
                SharesSoldId = model.SharesSoldId,
                DateOfPurchase = dateOfPurchase,
                DateOfSold = dateOfSold,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                CompanyName = model.CompanyName,
                HowMany = model.HowMany,
                PricePerShares = double.Round(model.PricePerShares, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PricePerSharesSold = double.Round(model.PricePerSharesSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Brokerage = model.Brokerage,
                TypeOfShares = model.TypeOfShares,
                MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PercentProfitOrLoss = model.PercentProfitOrLoss,
                Note = model.Note
            };

            return vm;
        }

        private static SharesSolds ChangeFromViewModelToModel(SharesSoldViewModel vm)
        {
            SharesSolds sharesSolds = new()
            {
                SharesSoldId = vm.SharesSoldId,
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd"),
                Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(vm.HowMany * int.Parse(vm.PricePerSharesSold), 2, MidpointRounding.AwayFromZero),
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = double.Round(double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero),
                PricePerSharesSold = double.Round(double.Parse(vm.PricePerSharesSold), 2, MidpointRounding.AwayFromZero),
                Brokerage = vm.Brokerage,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Account = vm.Account,
                TypeOfShares = vm.TypeOfShares,
                Note = vm.Note
            };

            sharesSolds.MoneyProfitOrLoss = sharesSolds.AmountSold - sharesSolds.Amount;

            double calculateMoneyProfitOrLoss = (sharesSolds.AmountSold / sharesSolds.Amount) - 1;

            sharesSolds.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

            return sharesSolds;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private static void ErrorHandling(ApplicationDbContext db, SharesSoldViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                CompanyOrInformation = vm.CompanyName,
                TypeOfTransaction = type,
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"{type} SÅLDA AKTIE: \r\nDatum: {vm.DateOfPurchase} \r\nImport: {importTrue} \r\nId: {vm.SharesSoldId} \r\nISIN: {vm.ISIN}."
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}