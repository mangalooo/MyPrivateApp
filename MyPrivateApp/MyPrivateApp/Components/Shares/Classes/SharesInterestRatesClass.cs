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

        public void Add(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                if (vm.Date == DateTime.MinValue) return;

                // Add new sold shares
                SharesInterestRates model = ChangeFromViewModelToModel(vm);

                try
                {
                    db.SharesInterestRates.Add(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Add fee {DateTime.Now}: Tax: {vm.Date}  Belopp:  {vm.TotalAmount}, Date: {vm.Date}. Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Avgifter: {DateTime.Now}: Tax: {vm.Date}, Belopp: {vm.TotalAmount}, Datum: {vm.Date}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            }
            else
                throw new Exception("Lägg till ränta: Hittar ingen data från formuläret eller ingen kontakt med databasen!");
        }

        public void Edit(ApplicationDbContext db, SharesInterestRatesViewModel vm)
        {
            if (vm != null && vm.InterestRatesId > 0 && db != null)
            {
                SharesInterestRates getDbModel = Get(db, vm.InterestRatesId);

                if (getDbModel != null)
                {
                    try
                    {
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Edit fee {DateTime.Now}: Tax: {vm.Date}, Belopp: {vm.TotalAmount}, Date: {vm.Date}. Error: {ex.Message}");

                        DateTime date = DateTime.Now;

                        SharesErrorHandlings sharesErrorHandling = new()
                        {
                            Date = $"{date.Year}-{date.Month}-{date.Day}",
                            ErrorMessage = $"Felmeddelande: {ex.Message}",
                            Note = $"Import: Nej, Ändra avgifter: {DateTime.Now}: Tax: {vm.Date}, Belopp: {vm.TotalAmount}, Datum: {vm.Date}"
                        };

                        db.SharesErrorHandlings.Add(sharesErrorHandling);
                        db.SaveChanges();
                    }
                }
                else
                    throw new Exception("Ändra räntan: Hittar inte aktien i databasen!");
            }
            else
                throw new Exception("Ändra räntan: Hittar ingen data från formuläret, ingen kontakt med databasen eller fel ISIN!");
        }

        public void Delete(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import)
        {
            if (vm != null && vm.InterestRatesId > 0 &&  db != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                SharesInterestRates model = ChangeFromViewModelToModel(vm);

                if (model != null)
                {
                    try
                    {
                        db.ChangeTracker.Clear();
                        db.SharesInterestRates.Remove(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Delete fee {DateTime.Now}: Tax: {vm.Date}, Belopp: {vm.TotalAmount}, Date: {model.Date}. Error: {ex.Message}");

                        DateTime date = DateTime.Now;

                        SharesErrorHandlings sharesErrorHandling = new()
                        {
                            Date = $"{date.Year}-{date.Month}-{date.Day}",
                            ErrorMessage = $"Felmeddelande: {ex.Message}",
                            Note = $"Import: {importTrue}, Ta bort avgifter: {DateTime.Now}: Tax: {vm.Date}, Belopp: {vm.TotalAmount}, Datum: {model.Date}"
                        };

                        db.SharesErrorHandlings.Add(sharesErrorHandling);
                        db.SaveChanges();
                    }
                }
            }
            else
                throw new Exception("Ta bort räntan: Hittar ingen data från formuläret eller ingen kontakt med databasen!");
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
                TotalAmount = model.TotalAmount,
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
                TotalAmount = double.Parse(model.AmountString),
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
                TotalAmount = vm.TotalAmount,
                Currency = vm.Currency,
                Note = vm.Note
            };

            return model;
        }
    }
}