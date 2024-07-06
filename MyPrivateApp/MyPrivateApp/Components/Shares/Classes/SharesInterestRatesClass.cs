using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesInterestRatesClass : ISharesInterestRatesClass
    {
        private static SharesInterestRates? Get(ApplicationDbContext db, int? id) => db.SharesInterestRates.Any(r => r.InterestRatesId == id) ?
                                                                                        db.SharesInterestRates.FirstOrDefault(r => r.InterestRatesId == id) :
                                                                                            throw new Exception("Räntan hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && vm.TotalAmount > 0)
                {
                    try
                    {
                        SharesInterestRates model = ChangeFromViewModelToModel(vm);

                        db.SharesInterestRates.Add(model);
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
                        ErrorHandling(db, vm, "Lägg till", import, "Du måste fylla i fälten: Datum och Belopp!");
                    else
                        return "Du måste fylla i fälten: Datum och Belopp!";
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

        public string Edit(ApplicationDbContext db, SharesInterestRatesViewModel vm)
        {
            if (vm != null && vm.InterestRatesId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && vm.TotalAmount > 0)
                {
                    try
                    {
                        SharesInterestRates? getDbModel = Get(db, vm.InterestRatesId);

                        if (getDbModel != null)
                        {
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.Account = vm.Account;
                            getDbModel.TypeOfTransaction = vm.TypeOfTransaction;
                            getDbModel.TotalAmount = vm.TotalAmount;
                            getDbModel.Currency = vm.Currency;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Räntan hittades inte i databasen!!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", false, ex.Message);
                    }
                }
                else
                    return "Du måste fylla i fälten: Datum och Belopp!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import)
        {
            if (vm != null && vm.InterestRatesId > 0 && db != null)
            {
                try
                {
                    SharesInterestRates model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.SharesInterestRates.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
                }
            }
            else
                return "Aktien hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public SharesInterestRatesViewModel ChangeFromModelToViewModel(SharesInterestRates model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesInterestRatesViewModel vm = new()
            {
                InterestRatesId = model.InterestRatesId,
                Date = date,
                Account = model.Account, 
                TypeOfTransaction = model.TypeOfTransaction,
                TotalAmount = double.Round(model.TotalAmount, 2, MidpointRounding.AwayFromZero),
                Currency = model.Currency,
                Note = model.Note
            };

            return vm;
        }

        public SharesInterestRatesViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesInterestRatesViewModel vm = new()
            {
                Account = model.AccountNumber,
                Currency = model.Currency,
                Date = date,
                TotalAmount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero),
                TypeOfTransaction = model.TypeOfTransaction
            };

            return vm;
        }

        private static SharesInterestRates ChangeFromViewModelToModel(SharesInterestRatesViewModel vm)
        {
            SharesInterestRates model = new()
            {
                InterestRatesId = vm.InterestRatesId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Account = vm.Account,
                TypeOfTransaction = vm.TypeOfTransaction,
                TotalAmount = double.Round(vm.TotalAmount, 2, MidpointRounding.AwayFromZero),
                Currency = vm.Currency,
                Note = vm.Note
            };

            return model;
        }

        private static void ErrorHandling(ApplicationDbContext db, SharesInterestRatesViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                TypeOfTransaction = vm.TypeOfTransaction,
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"{type} RÄNTA: \r\nDatum: {vm.Date} \r\nImport: {importTrue} \r\nId: {vm.InterestRatesId}"
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}