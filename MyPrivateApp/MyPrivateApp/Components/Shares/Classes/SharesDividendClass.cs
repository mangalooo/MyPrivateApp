using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesDividendClass : ISharesDividendClass
    {
        private static SharesDividend Get(int? id, ApplicationDbContext db) => db.SharesDividends.FirstOrDefault(r => r.DividendId == id);

        public void Add(ApplicationDbContext db, SharesDividendViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.Date == DateTime.MinValue) return;

            // Add new sold shares
            SharesDividend model = ChangeFromViewModelToModel(vm);

            model.TotalAmount = model.NumberOfShares * model.PricePerShare;

            try
            {
                db.SharesDividends.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sold shares add {DateTime.Now}: Company: {model.Company} Date: {model.Date} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Lägg till såld aktie: {DateTime.Now}: Företag: {model.Company} Datum: {model.Date}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public void Update(ApplicationDbContext db, SharesDividendViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.DividendId > 0)
            {
                SharesDividend dbModel = Get(vm.DividendId, db);

                SharesDividend model = ChangeFromViewModelToModel(vm);

                dbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                dbModel.AccountNumber = vm.AccountNumber;
                dbModel.TypeOfTransaction = vm.TypeOfTransaction;
                dbModel.Company = vm.Company;
                dbModel.NumberOfShares= vm.NumberOfShares;
                dbModel.PricePerShare= vm.PricePerShare;
                dbModel.TotalAmount= vm.NumberOfShares * vm.PricePerShare;
                dbModel.Currency = vm.Currency;
                dbModel.ISIN = vm.ISIN;
                dbModel.Note = vm.Note;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Edit sold shares {DateTime.Now}: Company: {model.Company} Date: {model.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Ändra såld aktie: {DateTime.Now}: Företag: {model.Company} Datum: {model.Date}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                return;
            }
        }

        public void Delete(ApplicationDbContext db, SharesDividendViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            SharesDividend model = ChangeFromViewModelToModel(vm);

            try
            {
                db.ChangeTracker.Clear();
                db.SharesDividends.Remove(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SoldSharesAdd {DateTime.Now}: Company: {model.Company} Date: {model.Date} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Ta bort såld: {DateTime.Now}: Företag: {model.Company} Datum: {model.Date}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesDividendViewModel vm = new()
            {
                DividendId = model.DividendId,
                Date = date,
                AccountNumber = model.AccountNumber,
                TypeOfTransaction = model.TypeOfTransaction,
                Company = model.Company,
                NumberOfShares = model.NumberOfShares,
                PricePerShare= model.PricePerShare,
                TotalAmount = model.TotalAmount,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Note = model.Note
            };

            return vm;
        }

        private static SharesDividend ChangeFromViewModelToModel(SharesDividendViewModel vm)
        {
            SharesDividend sharesDividend = new()
            {
                DividendId = vm.DividendId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                AccountNumber = vm.AccountNumber,
                TypeOfTransaction = vm.TypeOfTransaction,
                Company = vm.Company,
                NumberOfShares = vm.NumberOfShares,
                PricePerShare = vm.PricePerShare,
                TotalAmount = vm.TotalAmount,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Note = vm.Note
            };

            return sharesDividend;
        }
    }
}