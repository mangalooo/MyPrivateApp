
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.Shares.Classes.Interface;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesOtherImportsClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesOtherImportsClass> logger) : ISharesOtherImportsClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesOtherImportsClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm == null)
                return await HandleError(vm, "Lägg till", import, "Hittar ingen data från formuläret!");

            if (vm.Date == DateTime.MinValue)
                return await HandleError(vm, "Lägg till", import, "Ingen datum ifyllt!");

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                string importTrue = import ? "Ja" : "Nej";
                SharesOtherImports model = ChangeFromViewModelToModel(vm, importTrue);

                await db.SharesOtherImports.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesOtherShareImportViewModel vm)
        {
            if (vm == null || vm.OtherImportsId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue)
                return "Ingen datum ifyllt!";

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                SharesOtherImports? model = await db.SharesOtherImports.FirstOrDefaultAsync(r => r.OtherImportsId == vm.OtherImportsId)
                    ?? throw new Exception("Andra importer hittades inte i databasen!");

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra andra importer! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesOtherImports model)
        {
            if (model == null || model.OtherImportsId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.SharesOtherImports.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort andra importer! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        private static void EditModel(SharesOtherImports model, SharesOtherShareImportViewModel vm)
        {
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.Account = vm.Account;
            model.TypeOfTransaction = vm.TypeOfTransaction;
            model.Company = vm.Company;
            model.NumberOfShares = vm.NumberOfShares;
            model.PricePerShare = vm.PricePerShare;
            model.Amount = vm.Amount;
            model.Brokerage = vm.Brokerage;
            model.Currency = vm.Currency;
            model.ISIN = vm.ISIN;
            model.Note = vm.Note;
        }

        public SharesOtherShareImportViewModel ChangeFromModelToViewModel(SharesOtherImports model)
        {
            return new SharesOtherShareImportViewModel
            {
                OtherImportsId = model.OtherImportsId,
                Date = model.Date == null ? DateTime.MinValue : DateTime.MinValue,
                Account = model.Account,
                Company = model.Company,
                Currency = model.Currency,
                ISIN = model.ISIN,
                TypeOfTransaction = model.TypeOfTransaction,
                NumberOfShares = model.NumberOfShares,
                PricePerShare = model.PricePerShare,
                Amount = model.Amount,
                Brokerage = model.Brokerage,
                Note = model.Note
            };
        }

        public SharesOtherShareImportViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = ParseDate(model.Date);

            SharesOtherShareImportViewModel vm = new()
            {
                Account = model.AccountNumber,
                Company = model.CompanyOrInformation,
                Currency = model.Currency,
                Date = date,
                ISIN = model.ISIN,
                TypeOfTransaction = model.TypeOfTransaction
            };

            if (!string.IsNullOrEmpty(model.AmountString) && model.AmountString != "-")
                vm.Amount = double.Parse(model.AmountString);

            if (!string.IsNullOrEmpty(model.BrokerageString) && model.BrokerageString != "-")
                vm.Brokerage = double.Parse(model.BrokerageString);

            if (!string.IsNullOrEmpty(model.PricePerShareString) && model.PricePerShareString != "-")
                vm.PricePerShare = double.Parse(model.PricePerShareString);

            if (!string.IsNullOrEmpty(model.NumberOfSharesString) && model.NumberOfSharesString != "-")
            {
                string numberOfSharesString = model.NumberOfSharesString;
                double numberOfShares = numberOfSharesString.Contains("-") ? double.Parse(numberOfSharesString[1..]) : double.Parse(numberOfSharesString);

                vm.NumberOfShares = numberOfShares;

                if (numberOfSharesString.Contains('-'))
                    vm.Note = $"Antalet kom med ett - tecken: {model.NumberOfSharesString}";
            }

            return vm;
        }

        private static SharesOtherImports ChangeFromViewModelToModel(SharesOtherShareImportViewModel vm, string import)
        {
            return new SharesOtherImports
            {
                Account = vm.Account,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                TypeOfTransaction = vm.TypeOfTransaction,
                Company = vm.Company,
                NumberOfShares = vm.NumberOfShares,
                PricePerShare = vm.PricePerShare,
                Amount = vm.Amount,
                Brokerage = vm.Brokerage,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Note = $"Import: {import}\r\n. " + vm.Note
            };
        }

        private async Task<string> HandleError(SharesOtherShareImportViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesOtherShareImportViewModel? vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                if (vm == null)
                    throw new Exception("ErrorHandling: SharesDividendViewModel == null!");

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.Company,
                    TypeOfTransaction = vm.TypeOfTransaction,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} ANDRA IMPORTER: \r\nDatum: {vm.Date} \r\nImport: {importTrue}  \r\nISIN: {vm.ISIN}  \r\nId: {vm.OtherImportsId}. "
                };

                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("ErrorHandling: db == null!");
                await db.SharesErrorHandlings.AddAsync(sharesErrorHandling);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }
    }
}