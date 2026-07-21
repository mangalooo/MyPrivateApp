using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesInterestRatesClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesInterestRatesClass> logger) : ISharesInterestRatesClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesInterestRatesClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesInterestRatesViewModel vm, bool import)
        {
            if (vm == null)
                return await HandleError(null, "Lägg till", import, "Hittar ingen data från formuläret!");

            if (vm.Date == DateTime.MinValue && vm.TotalAmount <= 0)
                return await HandleError(null, "Lägg till", import, "Du måste fylla i fälten: Datum och Belopp!");

            try
            {
                // Connect to the database
                using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Add: db == null!");

                SharesInterestRates model = ChangeFromViewModelToModel(vm);
                await db.SharesInterestRates.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesInterestRatesViewModel vm)
        {
            if (vm == null || vm.InterestRatesId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            if (vm.Date == DateTime.MinValue && vm.TotalAmount <= 0)
                return "Du måste fylla i fälten: Datum och Belopp!";

            try
            {
                // Connect to the database
                using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Edit: db == null!");

                SharesInterestRates? model = await db.SharesInterestRates.FirstOrDefaultAsync(r => r.InterestRatesId == vm.InterestRatesId)
                    ?? throw new Exception("Räntan hittades inte i databasen!");

                if (model == null)
                    return "Hittar inte avgiften i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra räntan! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesInterestRates model)
        {
            if (model == null || model.InterestRatesId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            try
            {
                // Connect to the database
                using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issue
                db.SharesInterestRates.Remove(model);
                await db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort räntan! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string? date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return DateTime.MinValue;

            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;
        }

        public SharesInterestRatesViewModel ChangeFromModelToViewModel(SharesInterestRates model)
        {
            return new SharesInterestRatesViewModel
            {
                InterestRatesId = model.InterestRatesId,
                Date = model.Date != null ? ParseDate(model.Date) : DateTime.MinValue,
                Account = model.Account,
                TypeOfTransaction = model.TypeOfTransaction,
                TotalAmount = double.Round(model.TotalAmount, 2, MidpointRounding.AwayFromZero),
                Currency = model.Currency,
                Note = model.Note
            };
        }

        public SharesInterestRatesViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            return new SharesInterestRatesViewModel
            {
                Date = ParseDate(model.Date),
                Account = model.AccountNumber,
                Currency = model.Currency,
                TotalAmount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero),
                TypeOfTransaction = model.TypeOfTransaction,
            };
        }

        private static SharesInterestRates ChangeFromViewModelToModel(SharesInterestRatesViewModel vm)
        {
            return new SharesInterestRates
            {
                InterestRatesId = vm.InterestRatesId,
                Date = vm.Date != DateTime.MinValue ? vm.Date.ToString("yyyy-MM-dd") : null,
                Account = vm.Account,
                TypeOfTransaction = vm.TypeOfTransaction,
                TotalAmount = double.Round(vm.TotalAmount, 2, MidpointRounding.AwayFromZero),
                Currency = vm.Currency,
                Note = vm.Note
            };
        }

        private static void EditModel(SharesInterestRates model, SharesInterestRatesViewModel vm)
        {
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.Account = vm.Account;
            model.TypeOfTransaction = vm.TypeOfTransaction;
            model.TotalAmount = double.Round(vm.TotalAmount, 2, MidpointRounding.AwayFromZero);
            model.Currency = vm.Currency;
            model.Note = vm.Note;
        }

        private async Task<string> HandleError(SharesInterestRatesViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesInterestRatesViewModel? vm, string type, bool import, string errorMessage)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);

                DateTime date = DateTime.Now;
                string importTrue = import ? "Ja" : "Nej";

                // Connect to the database
                using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("ErrorHandling: db == null!");

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    TypeOfTransaction = vm.TypeOfTransaction,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} RÄNTA: " +
                       $"\r\nDatum: {vm.Date} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nId: {vm.InterestRatesId}"
                };

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