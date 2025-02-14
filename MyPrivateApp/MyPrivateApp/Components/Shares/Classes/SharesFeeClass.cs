
using Microsoft.IdentityModel.Tokens;
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

        public string Add(ApplicationDbContext db, SharesFeeViewModel vm, bool import, string soldDate)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && (vm.Tax > 0 || vm.Brokerage > 0 || vm.Fee > 0))
                {
                    try
                    {
                        SharesFee model = ChangeFromViewModelToModel(vm, soldDate);

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
                        ErrorHandling(db, vm, "Lägg till", import, "Ingen datum ifyllt eller någon av avfigt, skatt eller courtage måste vara mer än 0!");
                    else
                        return "Ingen datum ifyllt eller någon av avgift, skatt eller courtage måste vara mer än 0!";
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
                if (vm.Date != DateTime.MinValue && (vm.Tax > 0 || vm.Brokerage > 0 || vm.Fee > 0))
                {
                    try
                    {
                        SharesFee getDbModel = Get(db, vm.SharesFeeId);

                        if (getDbModel != null)
                        {
                            getDbModel.SharesFeeId = vm.SharesFeeId;
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.CompanyOrInformation = vm.CompanyOrInformation;
                            getDbModel.Fee = vm.Fee;
                            getDbModel.Tax = vm.Tax;
                            getDbModel.Brokerage = vm.Brokerage;
                            getDbModel.Note = vm.Note;

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
                    return "Ingen datum ifyllt eller någon av avfigt, skatt eller courtage måste vara mer än 0!";
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
                    SharesFee model = ChangeFromViewModelToModel(vm, "");

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
            SharesFeeViewModel fee = new()
            {
                SharesFeeId = model.SharesFeeId,
                CompanyOrInformation = model.CompanyOrInformation,
                Fee = model.Fee,
                Tax = model.Tax,
                Brokerage = model.Brokerage,
                Note = model.Note,
                Account = model.Account,
                TypeOfTransaction = model.TypeOfTransaction,
                ISIN = model.ISIN
            };

            if (!string.IsNullOrEmpty(model.Date))
            {
                DateTime date = DateTime.Parse(model.Date);
                fee.Date = date;
            }

            if (!string.IsNullOrEmpty(model.DateOfFee))
            {
                DateTime dateOfFee = DateTime.Parse(model.DateOfFee);
                fee.DateOfFee = dateOfFee;
            }

            return fee;
        }

        private static SharesFee ChangeFromViewModelToModel(SharesFeeViewModel vm, string soldDate)
        {
            SharesFee sharesFee = new()
            {
                SharesFeeId = vm.SharesFeeId,
                Date = soldDate,
                CompanyOrInformation = vm.CompanyOrInformation,
                Fee = double.Round(vm.Fee, 2, MidpointRounding.AwayFromZero),
                Tax = double.Round(vm.Tax, 2, MidpointRounding.AwayFromZero),
                Brokerage = double.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero),
                Note = vm.Note,
                DateOfFee = vm.DateOfFee.ToString("yyyy-MM-dd"),
                Account = vm.Account,
                TypeOfTransaction = vm.TypeOfTransaction,
                ISIN = vm.ISIN
            };

            return sharesFee;
        }

        public SharesFeeViewModel ChangeFromImportAddToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesFeeViewModel fee = new()
            {
                Date = date,
                Account = model.AccountNumber,
                CompanyOrInformation = model.CompanyOrInformation,
                TypeOfTransaction = model.TypeOfTransaction,
            };

            if (!string.IsNullOrEmpty(model.BrokerageString))
                fee.Brokerage = double.Parse(model.BrokerageString);

            switch (model.TypeOfTransaction)
            {
                case "Skatt":
                    fee.Tax = double.Parse(model.AmountString); 
                    break;
                case "Avgift":
                    fee.Fee = double.Parse(model.AmountString);
                    break;
            }

            return fee;
        }

        private static void ErrorHandling(ApplicationDbContext db, SharesFeeViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                CompanyOrInformation = vm.CompanyOrInformation,
                TypeOfTransaction = vm.TypeOfTransaction,
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"{type} AVGIFTER: " +
                       $"\r\nAvgiftsdatum: {vm.DateOfFee} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nAvgift: {vm.Fee} " +
                       $"\r\nSkatt: {vm.Tax} " +
                       $"\r\nCourtage: {vm.Brokerage} " +
                       $"\r\nKonto: {vm.Account} " +
                       $"\r\nISIN: {vm.ISIN}"
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}