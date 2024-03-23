using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesOtherImportsClass : ISharesOtherImportsClass
    {
        private static SharesOtherImports? Get(ApplicationDbContext db, string ISIN) => db.SharesOtherImports.Any(r => r.ISIN == ISIN) ?
                                                                                           db.SharesOtherImports.FirstOrDefault(r => r.ISIN == ISIN) :
                                                                                               throw new Exception("Andra aktien hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Company) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.NumberOfShares > 0 && vm.PricePerShare > 0 && vm.Brokerage > 0 && vm.Amount > 0)
                {
                    SharesOtherImports model = ChangeFromViewModelToModel(vm, importTrue);

                    try
                    {
                        db.SharesOtherImports.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Other imports add {DateTime.Now}: Company: {vm.Company} Date: {vm.Date} Error: {ex.Message}");

                        DateTime date = DateTime.Now;

                        SharesErrorHandlings sharesErrorHandling = new()
                        {
                            Date = $"{date.Year}-{date.Month}-{date.Day}",
                            ErrorMessage = $"Felmeddelande: {ex.Message}",
                            Note = $"Import: {importTrue}, Lägg till andra importer: {DateTime.Now}: Företag: {vm.Company} Datum: {vm.Date}"
                        };

                        db.SharesErrorHandlings.Add(sharesErrorHandling);
                        db.SaveChanges();
                    }

                    return string.Empty;
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, Summan och Courage!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
        }

        public string Edit(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm != null && vm.OtherImportsId > 0 && string.IsNullOrEmpty(vm.ISIN) && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Company) &&
                    vm.NumberOfShares > 0 && vm.PricePerShare > 0 && vm.Brokerage > 0)
                {
                    SharesOtherImports dbModel = Get(db, vm.ISIN);

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

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Other imports update {DateTime.Now}: Company: {vm.Company} Date: {vm.Date} Error: {ex.Message}");

                            DateTime date = DateTime.Now;
                            string importTrue = import ? "Ja" : "Nej";

                            SharesErrorHandlings sharesErrorHandling = new()
                            {
                                Date = $"{date.Year}-{date.Month}-{date.Day}",
                                ErrorMessage = $"Felmeddelande: {ex.Message}",
                                Note = $"Import: {importTrue}, Ändra andra importer: {DateTime.Now}: Företag: {vm.Company} Datum: {vm.Date}"
                            };

                            db.SharesErrorHandlings.Add(sharesErrorHandling);
                            db.SaveChanges();
                        }

                        return string.Empty;
                    }
                    else
                        return "Hittar inte aktien i databasen!";
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen eller fel ISIN!";
        }

        public string Delete(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm != null && vm.OtherImportsId > 0 && db != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                SharesOtherImports model = ChangeFromViewModelToModel(vm, importTrue);

                try
                {
                    db.ChangeTracker.Clear();
                    db.SharesOtherImports.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Other imports delete {DateTime.Now}: Company: {vm.Company} Date: {vm.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Ta bort andra importer: {DateTime.Now}: Företag: {vm.Company} Datum: {vm.Date}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                return string.Empty;
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
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
                Amount = double.Parse(model.AmountString),
                Brokerage = double.Parse(model.BrokerageString),
                Company = model.CompanyOrInformation,
                Currency = model.Currency,
                Date = date,
                ISIN = model.ISIN,
                NumberOfShares = int.Parse(model.NumberOfSharesString),
                TypeOfTransaction = model.TypeOfTransaction,
                PricePerShare = double.Parse(model.PricePerShareString)
            };

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
    }
}