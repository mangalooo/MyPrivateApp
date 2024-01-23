using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesOtherImportsClass : ISharesOtherImportsClass
    {
        private static SharesOtherImports Get(int? id, ApplicationDbContext db) => db.SharesOtherImports.FirstOrDefault(r => r.OtherImportsId == id);

        public void Add(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.Date == DateTime.MinValue) return;

            SharesOtherImports model = ChangeFromViewModelToModel(vm);

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
        }

        public void Update(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.OtherImportsId > 0)
            {
                SharesOtherImports dbModel = Get(vm.OtherImportsId, db);

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

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Ändra andra importer: {DateTime.Now}: Företag: {vm.Company} Datum: {vm.Date}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                return;
            }
        }

        public void Delete(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            SharesOtherImports model = ChangeFromViewModelToModel(vm);

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

        private static SharesOtherImports ChangeFromViewModelToModel(SharesOtherShareImportViewModel vm)
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
                Note = vm.Note
            };

            return model;
        }
    }
}