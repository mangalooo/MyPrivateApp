using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesFeeClass : ISharesFeeClass
    {
        private static SharesFee? Get(ApplicationDbContext db, int? id) => db.SharesFees.Any(r => r.SharesFeeId == id) ?
                                                                                db.SharesFees.FirstOrDefault(r => r.SharesFeeId == id) :
                                                                                    throw new Exception("Avgiften/skatten hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesFeeViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && (vm.Tax > 0 || vm.Brokerage > 0))
                {
                    try
                    {
                        SharesFee model = ChangeFromViewModelToModel(vm);

                        db.SharesFees.Add(model);
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
                        ErrorHandling(db, vm, "Lägg till", import, "Ingen datum ifyllt eller någon av skatt eller courtage måste vara mer än 0!");
                    else
                        return "Ingen datum ifyllt eller någon av skatt eller courtage måste vara mer än 0!";
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

        public string Edit(ApplicationDbContext db, SharesFeeViewModel vm)
        {
            if (vm != null && vm.SharesFeeId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && (vm.Tax > 0 || vm.Brokerage > 0))
                {
                    try
                    {
                        SharesFee getDbModel = Get(db, vm.SharesFeeId);

                        if (getDbModel != null)
                        {
                            getDbModel.SharesFeeId = vm.SharesFeeId;
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.Tax = vm.Tax;
                            getDbModel.Brokerage = vm.Brokerage;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", false, ex.Message);
                    }
                }
                else
                    return "Ingen datum ifyllt eller någon av skatt eller courtage måste vara mer än 0!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, SharesFeeViewModel vm, bool import)
        {
            if (vm != null && vm.SharesFeeId > 0 && db != null)
            {
                try
                {
                    SharesFee model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.SharesFees.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
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

        private static void ErrorHandling(ApplicationDbContext db, SharesFeeViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"{type} AVGIFTER: {DateTime.Now}: \r\nImport: {importTrue} \r\nTax: {vm.Tax} \r\nCourtage: {vm.Brokerage} \r\nDatum: {vm.Date}"
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}