using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesErrorHandlingClass
    {
        private static SharesErrorHandlings Get(ApplicationDbContext db, int? id) => db.SharesErrorHandlings.FirstOrDefault(r => r.ErrorHandlingsId == id);

        public static void Edit(ApplicationDbContext db, SharesErrorHandlingViewModel vm)
        {
            if (vm != null)
            {
                SharesErrorHandlings getDbModel = Get(db, vm.ErrorHandlingsId);

                try
                {
                    if (getDbModel != null)
                    {
                        getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                        getDbModel.ErrorMessage = vm.ErrorMessage;
                        getDbModel.Note = vm.Note;

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Edit error handling datum: {vm.Date}, Note: {vm.Note} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Ta bort felhandtering: Datum: {vm.Date}, Anteckningar: {vm.Note} Fel hantering: {vm.ErrorMessage}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
        }

        private static SharesErrorHandlings ChangeFromViewModelToModel(SharesErrorHandlingViewModel vm)
        {
            SharesErrorHandlings model = new()
            {
                ErrorHandlingsId = vm.ErrorHandlingsId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                ErrorMessage = vm.ErrorMessage,
                Note = vm.Note
            };

            return model;
        }

        public static SharesErrorHandlingViewModel ChangeFromModelToViewModel(SharesErrorHandlings model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesErrorHandlingViewModel vm = new()
            {
                ErrorHandlingsId = model.ErrorHandlingsId,
                Date = date,
                ErrorMessage = model.ErrorMessage,
                Note = model.Note
            };

            return vm;
        }

        public static void Delete(ApplicationDbContext db, SharesErrorHandlingViewModel vm)
        {
            SharesErrorHandlings model = ChangeFromViewModelToModel(vm);

            if (model != null)
            {
                try
                {
                    db.ChangeTracker.Clear();
                    db.SharesErrorHandlings.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DeleteErrorHandling {DateTime.Now}: Date: {model.Date} Error handling: {model.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Ta bort felhandtering: {DateTime.Now}:  Datum: {model.Date} Fel hantering: {model.ErrorMessage} \n Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
        }
    }
}
