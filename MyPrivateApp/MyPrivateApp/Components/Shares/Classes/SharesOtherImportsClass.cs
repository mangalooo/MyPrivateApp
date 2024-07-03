using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesOtherImportsClass : ISharesOtherImportsClass
    {
        private static SharesOtherImports? Get(ApplicationDbContext db, int? id) => db.SharesOtherImports.Any(r => r.OtherImportsId == id) ?
                                                                                           db.SharesOtherImports.FirstOrDefault(r => r.OtherImportsId == id) :
                                                                                               throw new Exception("Andra aktien hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                if (vm.Date != DateTime.MinValue)
                {
                    try
                    {
                        SharesOtherImports model = ChangeFromViewModelToModel(vm, importTrue);

                        db.SharesOtherImports.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Lägg till", import, ex.Message);
                    }

                    return string.Empty;
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Lägg till", import, "Ingen datum ifyllt!");
                    else
                        return "Ingen datum ifyllt!";
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

        public string Edit(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm != null && vm.OtherImportsId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue)
                {
                    try
                    {
                        SharesOtherImports dbModel = Get(db, vm.OtherImportsId);

                        if (dbModel != null)
                        {
                            dbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            dbModel.Account = vm.Account;
                            dbModel.TypeOfTransaction = vm.TypeOfTransaction;
                            dbModel.Company = vm.Company;
                            dbModel.NumberOfShares = vm.NumberOfShares;
                            dbModel.PricePerShare = vm.PricePerShare;
                            dbModel.Amount = vm.Amount;
                            dbModel.Currency = vm.Currency;
                            dbModel.ISIN = vm.ISIN;
                            dbModel.Brokerage = vm.Brokerage;
                            dbModel.Note = vm.Note;

                            db.SaveChanges();

                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", import, ex.Message);
                    }
                }
                else
                    return "Ingen datum ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm != null && vm.OtherImportsId > 0 && db != null)
            {
                try
                {
                    string importTrue = import ? "Ja" : "Nej";

                    SharesOtherImports model = ChangeFromViewModelToModel(vm, importTrue);

                    db.ChangeTracker.Clear();
                    db.SharesOtherImports.Remove(model);
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

        public SharesOtherShareImportViewModel ChangeFromModelToViewModel(SharesOtherImports model)
        {
            SharesOtherShareImportViewModel vm = new()
            {
                OtherImportsId = model.OtherImportsId,
                Date = DateTime.Parse(model.Date),
                Account = model.Account,
                TypeOfTransaction = model.TypeOfTransaction,
                Company = model.Company,
                NumberOfShares = model.NumberOfShares,
                PricePerShare = model.PricePerShare,
                Amount = model.Amount,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Brokerage = model.Brokerage,
                Note = model.Note
            };

            return vm;
        }

        public SharesOtherShareImportViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesOtherShareImportViewModel vm = new()
            {
                Account = model.AccountNumber,
                Company = model.CompanyOrInformation,
                Currency = model.Currency,
                Date = date,
                ISIN = model.ISIN,
                TypeOfTransaction = model.TypeOfTransaction
            };

            if (!string.IsNullOrEmpty(model.AmountString))
                vm.Amount = double.Parse(model.AmountString);

            if (!string.IsNullOrEmpty(model.BrokerageString))
                vm.Brokerage = double.Parse(model.BrokerageString);            
            
            if (!string.IsNullOrEmpty(model.PricePerShareString))
                vm.PricePerShare = double.Parse(model.PricePerShareString);

            string numberOfSharesString = model.NumberOfSharesString;
            int numberOfShares = numberOfSharesString.Contains('-') ? int.Parse(numberOfSharesString[1..]) : int.Parse(numberOfSharesString);

            vm.NumberOfShares = numberOfShares;

            if (numberOfSharesString.Contains('-'))
                vm.Note = $"Antalet kom med ett - tecken: {model.NumberOfSharesString}";

            return vm;
        }

        private static SharesOtherImports ChangeFromViewModelToModel(SharesOtherShareImportViewModel vm, string import)
        {
            SharesOtherImports model = new()
            {
                OtherImportsId = vm.OtherImportsId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Account = vm.Account,
                TypeOfTransaction = vm.TypeOfTransaction,
                Company = vm.Company,
                NumberOfShares = vm.NumberOfShares,
                PricePerShare = vm.PricePerShare,
                Amount = vm.Amount,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Brokerage = vm.Brokerage,
                Note = $"Import: {import}" + vm.Note
            };

            return model;
        }

        private static void ErrorHandling(ApplicationDbContext db, SharesOtherShareImportViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"{type} ANDRA IMPORTER: \r\nImport: {importTrue} \r\nFöretag: {vm.Company} \r\nDatum: {vm.Date} \r\nId: {vm.OtherImportsId}. "
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}