using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.HelpClasses
{
    public class SharesFeeHelpClass
    {
        public static void Create(ApplicationDbContext db, SharesFeeViewModel vm)
        {
            if (vm.Date == DateTime.MinValue) return;

            // Add new sold shares
            SharesFee model = ChangeFromViewModelToModel(vm);

            try
            {
                db.SharesFees.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddFee {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Date: {vm.Date} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Avgifter: {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Datum: {vm.Date} \n Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public static void Edit(ApplicationDbContext db, SharesFeeViewModel vm)
        {
            if (vm != null)
            {
                SharesFee getDbModel = Get(db, vm.SharesFeeId);

                try
                {
                    if (getDbModel != null)
                    {
                        getDbModel.SharesFeeId = vm.SharesFeeId;
                        getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                        getDbModel.Tax = vm.Tax;
                        getDbModel.Brokerage = vm.Brokerage;

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EditTaxAndBrokerage {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Date: {vm.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Ändra avgifter: {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Datum: {vm.Date} \n Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
        }

        private static SharesFee Get(ApplicationDbContext db, int? id) => db.SharesFees.FirstOrDefault(r => r.SharesFeeId == id);

        private static SharesFee ChangeFromViewModelToModel(SharesFeeViewModel vm)
        {
            SharesFee sharesFee = new()
            {
                SharesFeeId = vm.SharesFeeId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Tax = vm.Tax,
                Brokerage = vm.Brokerage,
                Note = vm.Note
            };

            return sharesFee;
        }

        public static SharesFeeViewModel ChangeFromModelToViewModel(SharesFee model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesFeeViewModel fee = new()
            {
                SharesFeeId = model.SharesFeeId,
                Date = date,
                Tax = model.Tax,
                Brokerage = model.Brokerage,
                Note = model.Note
            };

            return fee;
        }

        public static void Delete(ApplicationDbContext db, SharesFeeViewModel vm)
        {
            SharesFee model = ChangeFromViewModelToModel(vm);

            if (model != null)
            {
                try
                {
                    db.ChangeTracker.Clear();
                    db.SharesFees.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DeleteFee {DateTime.Now}: Tax: {model.Tax} Courtage: {model.Brokerage} Date: {model.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Ta bort avgifter: {DateTime.Now}: Tax: {model.Tax} Courtage: {model.Brokerage} Datum: {model.Date} \n Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
        }
    }
}