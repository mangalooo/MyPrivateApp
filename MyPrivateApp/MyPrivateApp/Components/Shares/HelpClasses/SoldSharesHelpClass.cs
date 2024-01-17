using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.HelpClasses
{
    public class SoldSharesHelpClass
    {
        public static void CreateOrUpdatePurchasedShares(ApplicationDbContext db, SharesSoldViewModel sharesSolds)
        {
            // Update sold shares
            if (sharesSolds.SharesSoldId > 0)
            {
                SharesSolds model = SetChanges(db, sharesSolds);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EditSold {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Ändra såld aktie: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                return;
            }

            if (sharesSolds.DateOfPurchase == DateTime.MinValue) return;

            // Add new sold shares
            SharesSolds sharesSoldModel = ChangesSharesSoldToModel(sharesSolds);

            try
            {
                db.SharesSolds.Add(sharesSoldModel);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SoldSharesAdd {DateTime.Now}: Company: {sharesSoldModel.CompanyName} Date: {sharesSoldModel.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Såld lägg till: {DateTime.Now}: Företag: {sharesSoldModel.CompanyName} Datum: {sharesSoldModel.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public static SharesSolds SetChanges(ApplicationDbContext db, SharesSoldViewModel sharesSolds)
        {
            SharesSolds getDbSharesSoldModel;
            SharesSolds changesToModel;

            getDbSharesSoldModel = GetSharesSolds(sharesSolds.SharesSoldId, db);

            changesToModel = ChangesSharesSoldToModel(sharesSolds);

            getDbSharesSoldModel.DateOfPurchase = changesToModel.DateOfPurchase;
            getDbSharesSoldModel.Amount = changesToModel.HowMany * changesToModel.PricePerShares;
            getDbSharesSoldModel.DateOfSold = changesToModel.DateOfSold;
            getDbSharesSoldModel.AmountSold = changesToModel.AmountSold;
            getDbSharesSoldModel.CompanyName = changesToModel.CompanyName;
            getDbSharesSoldModel.HowMany = changesToModel.HowMany;
            getDbSharesSoldModel.PricePerShares = changesToModel.PricePerShares;
            getDbSharesSoldModel.PricePerSharesSold = changesToModel.PricePerSharesSold;
            getDbSharesSoldModel.Currency = changesToModel.Currency;
            getDbSharesSoldModel.ISIN = changesToModel.ISIN;
            getDbSharesSoldModel.Account = changesToModel.Account;
            getDbSharesSoldModel.Brokerage = changesToModel.Brokerage;
            getDbSharesSoldModel.TypeOfShares = changesToModel.TypeOfShares;
            getDbSharesSoldModel.MoneyProfitOrLoss = changesToModel.MoneyProfitOrLoss;
            getDbSharesSoldModel.PercentProfitOrLoss = changesToModel.PercentProfitOrLoss;
            getDbSharesSoldModel.Note = changesToModel.Note;

            return changesToModel;
        }

        private static SharesSolds GetSharesSolds(int? id, ApplicationDbContext db) => db.SharesSolds.FirstOrDefault(r => r.SharesSoldId == id);

        public static SharesSolds ChangesSharesSoldToModel(SharesSoldViewModel vm)
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

        public static SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model)
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

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";
    }
}