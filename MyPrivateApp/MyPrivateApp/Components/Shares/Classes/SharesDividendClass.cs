
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.Shares.Classes.Interface;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesDividendClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesDividendClass> logger) : ISharesDividendClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesDividendClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesDividendViewModel vm, bool import)
        {
            try
            {
                if (vm == null)
                    return await HandleError(null, "Lägg till", import, "Hittar ingen data från formuläret!");

                if (IsImportantFieldsSet(vm))
                    return await HandleError(vm, "Lägg till", import, "Du måste fylla i fälten: Inköpsdatum, Företag, ISIN, Antal och Pris per aktie.");

                SharesDividend model = ChangeFromViewModelToModel(vm);

                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                await db.SharesDividends.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesDividendViewModel vm)
        {
            try
            {
                if (vm == null)
                    return "Hittar ingen data från formuläret!";

                if (IsImportantFieldsSet(vm))
                    return "Du måste fylla i fälten: Inköpsdatum, Företag, ISIN, Antal och Pris per aktie.";

                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                SharesDividend? model = await db.SharesDividends.FirstOrDefaultAsync(r => r.DividendId == vm.DividendId) 
                    ?? throw new Exception("Utdelningen hittades inte i databasen!");
                if (model == null)
                    return "Hittar inte utdelningen i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra utdelningen! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesDividend model)
        {
            if (model == null || model.DividendId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.SharesDividends.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort utdelningen! Felmeddelande: {ex.Message}";
            }
        }

        private static bool IsImportantFieldsSet(SharesDividendViewModel vm)
        {
            return vm == null
                ? throw new Exception("IsImportantFieldsSet: vm == null!")
                : vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Company) && string.IsNullOrEmpty(vm.ISIN) &&
                    vm.NumberOfShares < 0 && string.IsNullOrEmpty(vm.PricePerShare);
        }

        private static void EditModel(SharesDividend model, SharesDividendViewModel vm)
        {
            double pricePerShare = double.TryParse(vm.PricePerShare, out var parsedPrice) ? parsedPrice : 0.0;

            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.AccountNumber = vm.AccountNumber;
            model.TypeOfTransaction = vm.TypeOfTransaction;
            model.Company = vm.Company;
            model.NumberOfShares = vm.NumberOfShares;
            model.PricePerShare = pricePerShare;
            model.TotalAmount = pricePerShare * vm.NumberOfShares;
            model.Currency = vm.Currency;
            model.ISIN = vm.ISIN;
            model.Note = vm.Note;
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model)
        {
            return new SharesDividendViewModel
            {
                DividendId = model.DividendId,
                Date = model.Date != null ? ParseDate(model.Date) : DateTime.MinValue,
                AccountNumber = model.AccountNumber,
                TypeOfTransaction = model.TypeOfTransaction,
                Company = model.Company,
                NumberOfShares = model.NumberOfShares,
                PricePerShare = model.PricePerShare.ToString("#,##0.00"),
                TotalAmount = model.TotalAmount.ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Note = model.Note
            };
        }

        public SharesDividendViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            // Parse NumberOfShares
            double numberOfShares = double.TryParse(model.NumberOfSharesString, out var parsedNumberOfShares) ? parsedNumberOfShares : 0.0;
            // Parse PricePerShare
            double pricePerShare = double.TryParse(model.PricePerShareString, out var parsedPricePerShare) ? parsedPricePerShare : 0.0;
            // Calculate total amount
            double totalAmount = numberOfShares * pricePerShare;

            return new SharesDividendViewModel
            {
                Date = model.Date != null ? ParseDate(model.Date) : DateTime.MinValue,
                AccountNumber = model.AccountNumber,
                TypeOfTransaction = model.TypeOfTransaction,
                Company = model.CompanyOrInformation,
                NumberOfShares = numberOfShares,
                PricePerShare = pricePerShare.ToString("#,##0.00"),
                TotalAmount = totalAmount.ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN
            };
        }

        public SharesDividend ChangeFromViewModelToModel(SharesDividendViewModel vm)
        {
            double PricePerShare = double.TryParse(vm.PricePerShare, out var pricePerShare) ? pricePerShare : 0.0;

            return new SharesDividend
            {
                DividendId = vm.DividendId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                AccountNumber = vm.AccountNumber,
                TypeOfTransaction = vm.TypeOfTransaction,
                Company = vm.Company,
                NumberOfShares = vm.NumberOfShares,
                PricePerShare = PricePerShare,
                TotalAmount = PricePerShare * vm.NumberOfShares,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Note = vm.Note
            };
        }

        private async Task<string> HandleError(SharesDividendViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesDividendViewModel? vm, string type, bool import, string errorMessage)
        {
            try
            {

                if (vm == null)
                    throw new Exception("ErrorHandling: SharesDividendViewModel == null!");

                DateTime date = DateTime.Now;
                string importTrue = import ? "Ja" : "Nej";

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TypeOfTransaction = type,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = vm == null ? null : $"{type} UTDELNING: \r\nDatum: {vm.Date} \r\nImport: {importTrue}  \r\nISIN: {vm.ISIN} \r\nId: {vm.DividendId}. "
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