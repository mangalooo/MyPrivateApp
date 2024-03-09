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
                Console.WriteLine($"Dividend add {DateTime.Now}: Company: {model.Company} Date: {model.Date} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Lägg till utdelning: {DateTime.Now}: Företag: {model.Company} Datum: {model.Date}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public void Edit(ApplicationDbContext db, SharesDividendViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.DividendId > 0)
            {
                SharesDividend dbModel = Get(vm.DividendId, db);
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
                    Console.WriteLine($"Edit dividend {DateTime.Now}: Company: {vm.Company} Date: {vm.Date} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Ändra utdelning: {DateTime.Now}: Företag: {vm.Company} Datum: {vm.Date}"
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
                Console.WriteLine($"Delete dividend {DateTime.Now}: Company: {model.Company} Date: {model.Date} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Ta bort utdelning: {DateTime.Now}: Företag: {model.Company} Datum: {model.Date}"
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

        public SharesDividendViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesDividendViewModel vm = new()
            {
                Date = date,
                Company = model.CompanyOrInformation,
                NumberOfShares = int.Parse(model.NumberOfSharesString),
                PricePerShare = double.Parse(model.PricePerShareString),
                Brokerage = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                AccountNumber = model.AccountNumber,
                TotalAmount = double.Parse(model.AmountString),
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