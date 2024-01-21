using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesFeeClass : ISharesFeeClass
    {
        private static SharesFee Get(ApplicationDbContext db, int? id) => db.SharesFees.FirstOrDefault(r => r.SharesFeeId == id);

        public void Add(ApplicationDbContext db, SharesFeeViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

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
                Console.WriteLine($"Add fee {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Date: {vm.Date} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage =  $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Avgifter: {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Datum: {vm.Date}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public void Edit(ApplicationDbContext db, SharesFeeViewModel vm)
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
                    Console.WriteLine($"Edit fee {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Date: {vm.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: Nej, Ändra avgifter: {DateTime.Now}: Tax: {vm.Tax} Courtage: {vm.Brokerage} Datum: {vm.Date}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
        }

        public void Delete(ApplicationDbContext db, SharesFeeViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

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
                    Console.WriteLine($"Delete fee {DateTime.Now}: Tax: {model.Tax} Courtage: {model.Brokerage} Date: {model.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Ta bort avgifter: {DateTime.Now}: Tax: {model.Tax} Courtage: {model.Brokerage} Datum: {model.Date}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
        }

        public SharesFeeViewModel ChangeFromModelToViewModel(SharesFee model)
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
    }
}