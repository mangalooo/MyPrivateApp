
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesErrorHandlingClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesErrorHandlingClass> logger) : ISharesErrorHandlingClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesErrorHandlingClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Edit(SharesErrorHandlingViewModel vm)
        {
            try
            {
                if (vm == null || vm.ErrorHandlingsId <= 0)
                    return "Hittar ingen data från formuläret!";

                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                SharesErrorHandlings? model = await db.SharesErrorHandlings.FirstOrDefaultAsync(r => r.ErrorHandlingsId == vm.ErrorHandlingsId)
                    ?? throw new Exception("Falhanteringen hittades inte i databasen!");

                if (model == null)
                    return "Felhanteringen hittades inte i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;

            }
            catch (Exception ex)
            {
                string message = $"Ändra felhandtering: \r\nDatum: {vm.Date.ToString()[..10]} \r\nAnteckningar: {vm.Note} \r\nFel hantering: {vm.ErrorMessage}!";
                _logger.LogError(ex, message);
                
                return $"Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(SharesErrorHandlings model)
        {
            if (model == null)
                return "Hittar ingen data från modellen!";

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear();
                db.Remove(model);
                await db.SaveChangesAsync();

                string message = $"Ta bort felhandtering: \r\nDatum: {model.Date?.ToString()[..10] ?? DateTime.MinValue.ToString()} " +
                    $"\r\nAnteckningar: {model.Note ?? string.Empty} \r\nFel hantering: {model.ErrorMessage ?? string.Empty}!";
                _logger.LogInformation(message);

                return string.Empty;
            }
            catch (Exception ex)
            {
                string message = $"Ta bort felhandtering: \r\nDatum: {model?.Date?.ToString()[..10] ?? DateTime.MinValue.ToString()} " +
                    $"\r\nAnteckningar: {model?.Note ?? string.Empty} \r\nFel hantering: {model?.ErrorMessage ?? string.Empty}!";
                _logger.LogError(ex, message);
                
                return $"Felmeddelande: {ex.Message}";
            }
        }

        private static void EditModel(SharesErrorHandlings model, SharesErrorHandlingViewModel vm)
        {
            model.Date = vm.Date != DateTime.MinValue ? vm.Date.ToString("yyyy-MM-dd") : string.Empty;
            model.CompanyOrInformation = vm.CompanyOrInformation?.Trim() ?? string.Empty;
            model.TypeOfTransaction = vm.TypeOfTransaction?.Trim() ?? string.Empty;
            model.ErrorMessage = vm.ErrorMessage?.Trim() ?? string.Empty;
            model.Note = vm.Note?.Trim() ?? string.Empty;
        }

        public SharesErrorHandlingViewModel ChangeFromModelToViewModel(SharesErrorHandlings model)
        {
            return new SharesErrorHandlingViewModel
            {
                ErrorHandlingsId = model.ErrorHandlingsId,
                Date = model.Date != null && DateTime.TryParse(model.Date, out DateTime parsedDate) ? parsedDate : DateTime.MinValue,
                CompanyOrInformation = model.CompanyOrInformation?.Trim(),
                TypeOfTransaction = model.TypeOfTransaction?.Trim(),
                ErrorMessage = model.ErrorMessage?.Trim(),
                Note = model.Note?.Trim()
            };
        }
    }
}