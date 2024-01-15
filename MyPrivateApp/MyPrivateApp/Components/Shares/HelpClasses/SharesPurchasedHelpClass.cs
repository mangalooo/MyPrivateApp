using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.HelpClasses
{
    public class SharesPurchasedHelpClass
    {
        private static SharesPurchaseds GetSharesPurchased(ApplicationDbContext db, int? id) => db.SharesPurchaseds.FirstOrDefault(r => r.SharesPurchasedId == id);

        public static void CreateOrUpdatePurchasedShares(ApplicationDbContext db, SharesPurchasedViewModel sharesPurchased)
        {
            // Update hunting
            if (sharesPurchased.SharesPurchasedId > 0)
            {
                SharesPurchaseds c = db.SharesPurchaseds.FirstOrDefault(p => p.SharesPurchasedId == sharesPurchased.SharesPurchasedId);

                if (c is null) return;

                SharesPurchaseds model = SetChanges(db, sharesPurchased);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EditPurchasedShares {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Ändra köp: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                return;
            }

            if (sharesPurchased.SharesPurchasedId != 0) return;

            // Add new hunting
            SharesPurchaseds changesToModel = ChangesSharesPurchasedToModel(sharesPurchased);

            try
            {
                db.SharesPurchaseds.Add(changesToModel);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddShares {DateTime.Now}: Company: {changesToModel.CompanyName} Date: {changesToModel.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Köp: {DateTime.Now}: Företag: {changesToModel.CompanyName} Datum: {changesToModel.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }

            return;
        }

        public static SharesPurchaseds SetChanges(ApplicationDbContext db, SharesPurchasedViewModel sharesPurchased)
        {
            SharesPurchaseds getDbSharesPurchasedsModel;
            SharesPurchaseds changesToModel;

            getDbSharesPurchasedsModel = GetSharesPurchased(db, sharesPurchased.SharesPurchasedId);

            changesToModel = ChangesSharesPurchasedToModel(sharesPurchased);

            getDbSharesPurchasedsModel.DateOfPurchase = changesToModel.DateOfPurchase;
            getDbSharesPurchasedsModel.CompanyName = changesToModel.CompanyName;
            getDbSharesPurchasedsModel.HowMany = changesToModel.HowMany;
            getDbSharesPurchasedsModel.PricePerShares = changesToModel.PricePerShares;
            getDbSharesPurchasedsModel.Brokerage = changesToModel.Brokerage;
            getDbSharesPurchasedsModel.Currency = changesToModel.Currency;
            getDbSharesPurchasedsModel.ISIN = changesToModel.ISIN;
            getDbSharesPurchasedsModel.Account = changesToModel.Account;
            getDbSharesPurchasedsModel.Amount = changesToModel.HowMany * changesToModel.PricePerShares;
            getDbSharesPurchasedsModel.TypeOfShares = changesToModel.TypeOfShares;
            getDbSharesPurchasedsModel.Note = changesToModel.Note;

            return changesToModel;
        }

        public static SharesPurchasedViewModel ChangeFromModelToViewModel(SharesPurchaseds model)
        {
            DateTime date = DateTime.Parse(model.DateOfPurchase);

            SharesPurchasedViewModel vm = new()
            {
                SharesPurchasedId = model.SharesPurchasedId,
                DateOfPurchase = date,
                CompanyName = model.CompanyName,
                HowMany = model.HowMany,
                PricePerShares = model.PricePerShares,
                Brokerage = model.Brokerage,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Amount = model.Amount,
                TypeOfShares = model.TypeOfShares,
                Note = model.Note
            };

            return vm;
        }

        public static SharesPurchaseds ChangesSharesPurchasedToModel(SharesPurchasedViewModel vm)
        {
            SharesPurchaseds sharesPurchased = new()
            {
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = vm.PricePerShares,
                Brokerage = vm.Brokerage,
                Amount = vm.HowMany * vm.PricePerShares,
                ISIN = vm.ISIN,
                Currency = vm.Currency,
                Account = vm.Account,
                TypeOfShares = vm.TypeOfShares,
                Note = "Köper " + vm.CompanyName + " aktier: " + "Datum: " + vm.DateOfPurchase.ToString().Substring(0, 10) + ", Hur många: " + vm.HowMany +
                        ", Pris per st: " + vm.PricePerShares + ", Summan: " + vm.HowMany * vm.PricePerShares + ", Courtage: " + vm.Brokerage + ". "
            };

            return sharesPurchased;
        }

        public static void DeleteSharesPurchased(ApplicationDbContext db, SharesPurchasedViewModel incomingModel)
        {
            SharesPurchaseds sharesPurchasedModel = ChangesSharesPurchasedToModel(incomingModel);

            try
            {
                db.ChangeTracker.Clear();
                db.SharesPurchaseds.Remove(sharesPurchasedModel);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeletePurchased {DateTime.Now}: Company: {sharesPurchasedModel.CompanyName} Date: {sharesPurchasedModel.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Ta bort: {DateTime.Now}: Företag: {sharesPurchasedModel.CompanyName} Datum: {sharesPurchasedModel.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }
    }
}