
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesErrorHandlingClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesErrorHandlingClass> logger, IMapper mapper) : ISharesErrorHandlingClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesErrorHandlingClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<SharesErrorHandlings?> Get(int? id)
        {
            if (id <= 0)
                throw new Exception("Get: Finns inget ID!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Get: db == null!");

            return await db.SharesErrorHandlings.FirstOrDefaultAsync(r => r.ErrorHandlingsId == id)
                ?? throw new Exception("Falhanteringen hittades inte i databasen!");
        }

        public async Task<string> Edit(SharesErrorHandlingViewModel vm)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                if (vm == null || vm.ErrorHandlingsId <= 0)
                    return "Hittar ingen data från formuläret!";

                SharesErrorHandlings? getDbModel = await Get(vm.ErrorHandlingsId);

                if (getDbModel == null)
                    return "Felhanteringen hittades inte i databasen!";

                _mapper.Map(vm, getDbModel);
                await db.SaveChangesAsync();

                return string.Empty;

            }
            catch (Exception ex)
            {
                string message = $"Ändra felhandtering: \r\nDatum: {vm.Date.ToString()[..10]} \r\nAnteckningar: {vm.Note} \r\nFel hantering: {vm.ErrorMessage}!";
                _logger.LogError(ex, message);
                return $"Ändra. Felmeddelande: {ex.Message}";
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
                return $"Ta bort. Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date.ToString()[..10]}");
        }

        public SharesErrorHandlings ChangeFromViewModelToModel(SharesErrorHandlingViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesErrorHandlings model = _mapper.Map<SharesErrorHandlings>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }

        public SharesErrorHandlingViewModel ChangeFromModelToViewModel(SharesErrorHandlings model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesErrorHandlingViewModel vm = _mapper.Map<SharesErrorHandlingViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }
    }
}