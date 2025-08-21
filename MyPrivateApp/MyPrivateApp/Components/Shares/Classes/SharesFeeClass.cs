
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesFeeClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesFeeClass> logger) : ISharesFeeClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesFeeClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesFeeViewModel vm, bool import)
        {
            try
            {
                if (vm == null)
                    return await HandleError(vm, "Lägg till", import, "Hittar ingen data från formuläret eller databasen!");

                if (vm.Date == DateTime.MinValue && (vm.Tax <= 0 || vm.Brokerage <= 0 || vm.Fee <= 0))
                    return await HandleError(vm, "Lägg till", import, "Ingen datum ifyllt eller någon av avfigt, skatt eller courtage måste vara mer än 0!");

                SharesFee model = ChangeFromViewModelToModel(vm);

                // Connect to the database
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                await db.SharesFees.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesFeeViewModel vm)
        {
            try
            {
                if (vm == null || vm.SharesFeeId <= 0)
                    return "Hittar ingen data från formuläret!";

                if (vm.Date == DateTime.MinValue && (vm.Tax <= 0 || vm.Brokerage <= 0 || vm.Fee <= 0))
                    return "Ingen datum ifyllt eller någon av avgift, skatt eller courtage måste vara mer än 0!";

                // Connect to the database
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                SharesFee? model = await db.SharesFees.FirstOrDefaultAsync(r => r.SharesFeeId == vm.SharesFeeId)
                    ?? throw new Exception("Avgiften/skatten hittades inte i databasen!");

                if (model == null)
                    return "Hittar inte avgiften i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;

            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra avgiften! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesFee model)
        {
            if (model == null || model.SharesFeeId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                // Connect to the database
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.SharesFees.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort avgiften! Felmeddelande: {ex.Message}";
            }
        }

        private static void EditModel(SharesFee model, SharesFeeViewModel vm)
        {
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.CompanyOrInformation = vm.CompanyOrInformation;
            model.Tax = Math.Round(vm.Tax, 2, MidpointRounding.AwayFromZero);
            model.Fee = Math.Round(vm.Fee, 2, MidpointRounding.AwayFromZero);
            model.Brokerage = Math.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero);
            model.Note = vm.Note;

            // Error information
            model.DateOfFee = vm.DateOfFee.ToString("yyyy-MM-dd");
            model.Account = vm.Account;
            model.TypeOfTransaction = vm.TypeOfTransaction;
            model.ISIN = vm.ISIN;
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesFeeViewModel ChangeFromModelToViewModel(SharesFee model)
        {
            return new SharesFeeViewModel
            {
                SharesFeeId = model.SharesFeeId,
                Date = ParseDate(model.Date ?? string.Empty),
                CompanyOrInformation = model.CompanyOrInformation,
                Tax = model.Tax,
                Fee = model.Fee,
                Brokerage = model.Brokerage,
                Note = model.Note,
                DateOfFee = ParseDate(model.DateOfFee ?? string.Empty),
                Account = model.Account,
                TypeOfTransaction = model.TypeOfTransaction,
                ISIN = model.ISIN
            };
        }

        public SharesFeeViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            SharesFeeViewModel vm = new()
            {
                Date = ParseDate(model.Date),
                CompanyOrInformation = model.CompanyOrInformation,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
            };

            switch (model.TypeOfTransaction)
            {
                case "Skatt":
                    vm.Tax = double.Parse(model.AmountString);
                    break;
                case "Avgift":
                    vm.Fee = double.Parse(model.AmountString);
                    break;
                case "Courtage":
                    vm.Brokerage = double.Parse(model.AmountString);
                    break;
            }

            return vm;
        }

        private static SharesFee ChangeFromViewModelToModel(SharesFeeViewModel vm)
        {
            return new SharesFee
            {
                Date = vm.Date.ToString("yyyy-MM-dd"),
                CompanyOrInformation = vm.CompanyOrInformation,
                Tax = Math.Round(vm.Tax, 2, MidpointRounding.AwayFromZero),
                Fee = Math.Round(vm.Fee, 2, MidpointRounding.AwayFromZero),
                Brokerage = Math.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero),
                Note = vm.Note,
                DateOfFee = vm.DateOfFee.ToString("yyyy-MM-dd"),
                Account = vm.Account,
                TypeOfTransaction = vm.TypeOfTransaction,
                ISIN = vm.ISIN,
            };
        }

        private async Task<string> HandleError(SharesFeeViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesFeeViewModel? vm, string type, bool import, string errorMessage)
        {
            try
            {
                if (vm == null)
                    throw new Exception("ErrorHandling: SharesSoldViewModel == null!");

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

                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("ErrorHandling: db == null!");

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