
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesErrorHandlingClass(ApplicationDbContext db, ILogger<SharesErrorHandlingClass> logger, IMapper mapper) : ISharesErrorHandlingClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesErrorHandlingClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<SharesErrorHandlings?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.SharesErrorHandlings.FirstOrDefaultAsync(r => r.ErrorHandlingsId == id)
                ?? throw new Exception("Falhanteringen hittades inte i databasen!");
        }

        public async Task<string> Edit(SharesErrorHandlingViewModel vm)
        {
            if (_db == null || vm == null || vm.ErrorHandlingsId <= 0)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                SharesErrorHandlings? getDbModel = await Get(vm.ErrorHandlingsId);

                if (getDbModel == null)
                    return "Felhanteringen hittades inte i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
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
            if (_db == null || model == null)
                return "Hittar ingen data från modellen eller ingen kontakt med databasen!";

            try
            {
                _db.ChangeTracker.Clear();
                _db.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                string message = $"Ta bort felhandtering: \r\nDatum: {model?.Date?.ToString()[..10]} \r\nAnteckningar: {model.Note} \r\nFel hantering: {model.ErrorMessage}!";
                _logger.LogError(ex, message);
                return $"Felmeddelande: {ex.Message}";
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