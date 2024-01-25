using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldClass : ISharesSoldClass
    {
        private static SharesSolds Get(int? id, ApplicationDbContext db) => db.SharesSolds.FirstOrDefault(r => r.SharesSoldId == id);

        public void Add(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.DateOfPurchase == DateTime.MinValue) return;

            // Add new sold shares
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

        public void Edit(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.SharesSoldId > 0)
            {
                SharesSolds dbModel = Get(vm.SharesSoldId, db);

                SharesSolds model = ChangeFromViewModelToModel(vm);

                dbModel.DateOfPurchase = model.DateOfPurchase;
                dbModel.Amount = model.HowMany * model.PricePerShares;
                dbModel.DateOfSold = model.DateOfSold;
                dbModel.AmountSold = model.AmountSold;
                dbModel.CompanyName = model.CompanyName;
                dbModel.HowMany = model.HowMany;
                dbModel.PricePerShares = model.PricePerShares;
                dbModel.PricePerSharesSold = model.PricePerSharesSold;
                dbModel.Currency = model.Currency;
                dbModel.ISIN = model.ISIN;
                dbModel.Account = model.Account;
                dbModel.Brokerage = model.Brokerage;
                dbModel.TypeOfShares = model.TypeOfShares;
                dbModel.MoneyProfitOrLoss = model.MoneyProfitOrLoss;
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

                return;
            }
        }

        public void Delete(ApplicationDbContext db, SharesSoldViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

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

        public SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model)
        {
            DateTime dateOfPurchase = DateTime.Parse(model.DateOfPurchase);
            DateTime dateOfSold = DateTime.Parse(model.DateOfSold);

            SharesSoldViewModel vm = new()
            {
                SharesSoldId = model.SharesSoldId,
                DateOfPurchase = dateOfPurchase,
                DateOfSold = dateOfSold,
                Amount = model.Amount,
                AmountSold = model.AmountSold,
                CompanyName = model.CompanyName,
                HowMany = model.HowMany,
                PricePerShares = model.PricePerShares,
                PricePerSharesSold = model.PricePerSharesSold,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Brokerage = model.Brokerage,
                TypeOfShares = model.TypeOfShares,
                MoneyProfitOrLoss = model.MoneyProfitOrLoss,
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
                Amount = vm.HowMany * vm.PricePerShares,
                AmountSold = vm.HowMany * vm.PricePerSharesSold,
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = vm.PricePerShares,
                PricePerSharesSold = vm.PricePerSharesSold,
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