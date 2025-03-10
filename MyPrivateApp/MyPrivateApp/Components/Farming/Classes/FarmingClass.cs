
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models.Farming;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Farming.Classes
{
    public class FarmingClass(ApplicationDbContext db, ILogger<FarmingClass> logger, IMapper mapper) : IFarmingClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<FarmingClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<FarmingsActive?> GetActive(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.FarmingsActive.FirstOrDefaultAsync(r => r.FarmingId == id)
                   ?? throw new Exception("Aktiv odlingen hittades inte i databasen!");
        }

        public async Task<FarmingsInactive?> GetInactive(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.FarmingsInactive.FirstOrDefaultAsync(r => r.FarmingId == id)
                   ?? throw new Exception("Inaktiv odlingen hittades inte i databasen!");
        }

        public async Task<string> Add(FarmingViewModels vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen aktiv odling i databasen!";

            if (string.IsNullOrEmpty(vm.Name) || string.IsNullOrEmpty(vm.Type))
                return "Du måste fylle i namn och typ!";

            try
            {
                FarmingsActive model = _mapper.Map<FarmingsActive>(vm);
                await _db.FarmingsActive.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ny odling!");
                return $"Gick inte att lägg till ny odling. Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> EditActive(FarmingViewModels vm)
        {
            if (vm == null || vm.FarmingId <= 0 || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (string.IsNullOrEmpty(vm.Name) || string.IsNullOrEmpty(vm.Type))
                return "Du måste fylle i namn och typ!";

            try
            {
                FarmingsActive? getDbModel = await GetActive(vm.FarmingId);

                if (getDbModel == null) return "Hittar inte aktiv odling i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra aktiv odling!");
                return $"Gick inte att ändra aktiv odling. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> EditInactive(FarmingViewModels vm)
        {
            if (vm == null || vm.FarmingId <= 0 || _db == null)
                return "Ändra: Hittar ingen data från formuläret eller ingen inaktiv odling i databasen!";

            if (string.IsNullOrEmpty(vm.Name) || string.IsNullOrEmpty(vm.Type))
                return "Du måste fylle i namn och typ!";

            try
            {
                FarmingsInactive? getDbModel = await GetInactive(vm.FarmingId);

                if (getDbModel == null) return "Hittar inte odlingen i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra inaktiv odling!");
                return $"Gick inte att ändra inaktiv odling! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Inactive(FarmingViewModels vm)
        {
            if (vm == null)
                return "Får ingen kontakt med databasen eller formuläret!";

            FarmingsInactive farmings = ((IFarmingClass)this).ChangeFromViewModelToModel<FarmingsInactive>(vm);
            farmings.InactiveDate = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                await _db.FarmingsInactive.AddAsync(farmings);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att inaktivera odlingen!");
                return $"Gick inte att inaktivera odlingen! Felmeddelande: {ex.Message}";
            }

            // Removes active farming
            await DeleteActive(vm);

            return string.Empty;
        }

        public async Task<string> DeleteActive(FarmingViewModels vm)
        {
            if (vm == null || vm.FarmingId <= 0)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                FarmingsActive model = ((IFarmingClass)this).ChangeFromViewModelToModel<FarmingsActive>(vm);

                _db.ChangeTracker.Clear();
                _db.FarmingsActive.Remove(model);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort aktiv odling.");
                return $"Gick inte att ta bort aktiv odling. Felmeddelande: {ex.Message}";
            }

            return string.Empty;
        }

        public async Task<string> DeleteInactive(FarmingViewModels vm)
        {
            if (vm == null || vm.FarmingId <= 0)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                FarmingsInactive model = ((IFarmingClass)this).ChangeFromViewModelToModel<FarmingsInactive>(vm);

                _db.ChangeTracker.Clear();
                _db.FarmingsInactive.Remove(model);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort inaktiv odling.");
                return $"Gick inte att ta bort inaktiv odling. Felmeddelande: {ex.Message}";
            }

            return string.Empty;
        }

        public FarmingViewModels ChangeFromModelToViewModel(FarmingsActive model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            FarmingViewModels farming = _mapper.Map<FarmingViewModels>(model);

            farming.PutSeedDate = ParseDate(model.PutSeedDate);
            farming.SetDate = ParseDate(model.SetDate);
            farming.TakeUpDate = ParseDate(model.TakeUpDate);

            return farming;
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Invalid date format: {date}");
        }

        public FarmingViewModels ChangeFromModelToViewModel(FarmingsInactive model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            FarmingViewModels farming = _mapper.Map<FarmingViewModels>(model);

            farming.PutSeedDate = ParseDate(model.PutSeedDate);
            farming.SetDate = ParseDate(model.SetDate);
            farming.TakeUpDate = ParseDate(model.TakeUpDate);

            return farming;
        }

        T IFarmingClass.ChangeFromViewModelToModel<T>(FarmingViewModels vm)
        {
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            // Use AutoMapper to map the ViewModel to the Model
            T model = _mapper.Map<T>(vm);

            // Additional custom mapping if needed
            if (model is FarmingsActive activeModel)
            {
                activeModel.PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd");
                activeModel.SetDate = vm.SetDate.ToString("yyyy-MM-dd");
                activeModel.TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd");
            }
            else if (model is FarmingsInactive inactiveModel)
            {
                inactiveModel.PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd");
                inactiveModel.SetDate = vm.SetDate.ToString("yyyy-MM-dd");
                inactiveModel.TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd");
            }

            return model;
        }
    }
}