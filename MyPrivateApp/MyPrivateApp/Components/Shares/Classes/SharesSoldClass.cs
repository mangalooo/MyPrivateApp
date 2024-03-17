using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldClass : ISharesSoldClass
    {
        private static SharesSolds? Get(ApplicationDbContext db, string ISIN) => db.SharesSolds.Any(r => r.ISIN == ISIN) ?
                                                                                    db.SharesSolds.FirstOrDefault(r => r.ISIN == ISIN) :
                                                                                        throw new Exception("Den sålda aktien hittades inte i databasen!");

        public void Add(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm != null && vm.DateOfPurchase == DateTime.MinValue && db != null)
            {
                SharesSolds model = ChangeFromViewModelToModel(vm);

                try
                {
                    db.SharesSolds.Add(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Sold shares add {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Lägg till såld aktie: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
            else
                throw new Exception("Lägg till: Hittar ingen data från formuläret eller datum ej i fyllt!");
        }

        public void Edit(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm != null && string.IsNullOrEmpty(vm.ISIN) && db != null)
            {
                SharesSolds? dbModel = Get(db, vm.ISIN);

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

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Edit sold shares {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                        DateTime date = DateTime.Now;

                        SharesErrorHandlings sharesErrorHandling = new()
                        {
                            Date = $"{date.Year}-{date.Month}-{date.Day}",
                            ErrorMessage = $"Felmeddelande: {ex.Message}",
                            Note = $"Import: {importTrue}, Ändra såld aktie: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase}"
                        };

                        db.SharesErrorHandlings.Add(sharesErrorHandling);
                        db.SaveChanges();
                    }
                }
                else
                    throw new Exception("Ändra: Den sålda aktien hittades inte i databasen!");
            }
            else
                throw new Exception("Ändra: Den sålda aktien hittades inte i databasen eller saknas data i formuläret!");
        }

        public void Delete(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm != null && db != null)
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
                    Console.WriteLine($"SoldSharesAdd {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Ta bort såld: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
            else
                throw new Exception("Ta bort: Den sålda aktien hittades inte i databasen!");
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
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero),
                CompanyName = model.CompanyName,
                HowMany = model.HowMany,
                PricePerShares = double.Round(model.PricePerShares, 2, MidpointRounding.AwayFromZero),
                PricePerSharesSold = double.Round(model.PricePerSharesSold, 2, MidpointRounding.AwayFromZero),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Brokerage = model.Brokerage,
                TypeOfShares = model.TypeOfShares,
                MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero),
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
                Amount = double.Round(vm.HowMany * vm.PricePerShares, 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(vm.HowMany * vm.PricePerSharesSold, 2, MidpointRounding.AwayFromZero),
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = double.Round(vm.PricePerShares, 2, MidpointRounding.AwayFromZero),
                PricePerSharesSold = double.Round(vm.PricePerSharesSold, 2, MidpointRounding.AwayFromZero),
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
    }
}