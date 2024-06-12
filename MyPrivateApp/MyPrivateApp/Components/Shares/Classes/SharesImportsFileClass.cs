
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesImportsFileClass : ISharesImportsFileClass
    {
        private static SharesImportsFile? Get(ApplicationDbContext db, int? id) => db.SharesImportsFiles.Any(r => r.SharesImportsFileId == id) ?
                                                                                            db.SharesImportsFiles.FirstOrDefault(r => r.SharesImportsFileId == id) :
                                                                                                throw new Exception("Hittar inte importen i databasen!");

        public string Add(ApplicationDbContext db, SharesImportsFileViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue)
                {
                    SharesImportsFile model = ChangeFromViewModelToModel(vm);

                    try
                    {
                        db.SharesImportsFiles.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        DateTime date = DateTime.Now;

                        SharesErrorHandlings sharesErrorHandling = new()
                        {
                            Date = $"{date.Year}-{date.Month}-{date.Day}",
                            ErrorMessage = $"Felmeddelande: {ex.Message}",
                            Note = $"Import: {importTrue}, Importerad fil: {DateTime.Now}: Filnamn: {vm.FileName}, Datum: {vm.Date}"
                        };

                        db.SharesErrorHandlings.Add(sharesErrorHandling);
                        db.SaveChanges();
                    }
                }
                else
                    return "Datum måste vara ifylld!";
            }
            else
                return "Lägg till import: Hittar ingen data från formuläret, datum mini eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, SharesImportsFileViewModel vm)
        {
            if (vm != null && db != null)
            {
                SharesImportsFile getDbModel = Get(db, vm.SharesImportsFileId);

                try
                {
                    if (getDbModel != null)
                    {
                        getDbModel.SharesImportsFileId = vm.SharesImportsFileId;
                        getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                        getDbModel.FileName = vm.FileName;
                        getDbModel.NumbersOfTransaction = vm.NumbersOfTransaction;
                        getDbModel.Errors = vm.Errors;
                        getDbModel.Note = vm.Note;

                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: Nej, Ändra importerad fil: {DateTime.Now}: Filnamn: {vm.FileName}, Datum: {vm.Date}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
            else
                return "Ändra import: Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, SharesImportsFileViewModel vm, bool import)
        {
            if (vm != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                SharesImportsFile model = ChangeFromViewModelToModel(vm);

                if (model != null)
                {
                    try
                    {
                        db.ChangeTracker.Clear();
                        db.SharesImportsFiles.Remove(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        DateTime date = DateTime.Now;

                        SharesErrorHandlings sharesErrorHandling = new()
                        {
                            Date = $"{date.Year}-{date.Month}-{date.Day}",
                            ErrorMessage = $"Felmeddelande: {ex.Message}",
                            Note = $"Import: {importTrue}, Ta bort avgifter: {DateTime.Now}: Filnamn: {vm.FileName}, Datum: {model.Date}"
                        };

                        db.SharesErrorHandlings.Add(sharesErrorHandling);
                        db.SaveChanges();
                    }
                }
            }
            else
                return "Ta bort import: Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public SharesImportsFileViewModel ChangeFromModelToViewModel(SharesImportsFile model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesImportsFileViewModel fee = new()
            {
                SharesImportsFileId = model.SharesImportsFileId,
                Date = date,
                FileName = model.FileName,
                NumbersOfTransaction = model.NumbersOfTransaction,
                Errors = model.Errors,
                Note = model.Note
            };

            return fee;
        }

        private static SharesImportsFile ChangeFromViewModelToModel(SharesImportsFileViewModel vm)
        {
            SharesImportsFile model = new()
            {
                SharesImportsFileId = vm.SharesImportsFileId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                FileName = vm.FileName,
                NumbersOfTransaction = vm.NumbersOfTransaction,
                Errors = vm.Errors,
                Note = vm.Note
            };

            return model;
        }
    }
}