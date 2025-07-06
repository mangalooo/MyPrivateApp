
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Farming;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels;

namespace MyPrivateApp.Components.Farming.Classes
{
    public class FarmingClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmingClass> logger, IMapper mapper) : IFarmingClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmingClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<string> Add(FarmingViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (string.IsNullOrEmpty(vm.Name) || string.IsNullOrEmpty(vm.Type))
                return "Du måste fylle i namn och typ!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                FarmingsActive model = _mapper.Map<FarmingsActive>(vm);

                await db.FarmingsActive.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

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
            if (vm == null || vm.FarmingId <= 0)
                return "Hittar ingen data från formuläret!";

            if (string.IsNullOrEmpty(vm.Name) || string.IsNullOrEmpty(vm.Type))
                return "Du måste fylle i namn och typ!";
            
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("EditActive: db == null!");

                // Fetch the entity in the same context to ensure tracking
                FarmingsActive? model = await db.FarmingsActive.FirstOrDefaultAsync(r => r.FarmingId == vm.FarmingId);
                if (model == null)
                    return "Hittar inte aktiv odling i databasen!";

                _mapper.Map(vm, model);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

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
            if (vm == null || vm.FarmingId <= 0)
                return "Ändra: Hittar ingen data från formuläret!";

            if (string.IsNullOrEmpty(vm.Name) || string.IsNullOrEmpty(vm.Type))
                return "Du måste fylle i namn och typ!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("EditInactive: db == null!");

                // Fetch the entity in the same context to ensure tracking
                FarmingsInactive? getDbModel = await db.FarmingsInactive.FirstOrDefaultAsync(r => r.FarmingId == vm.FarmingId);

                if (getDbModel == null)
                    return "Hittar inte odlingen i databasen!";

                _mapper.Map(vm, getDbModel);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

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
                return "Får ingen kontakt med formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Inactive: db == null!");

                FarmingsInactive model = ((IFarmingClass)this).ChangeFromViewModelToModel<FarmingsInactive>(vm);
                model.InactiveDate = DateTime.Now.ToString("yyyy-MM-dd");

                await db.FarmingsInactive.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                // Removes active farming
                FarmingsActive ActiveModel = ((IFarmingClass)this).ChangeFromViewModelToModel<FarmingsActive>(vm);
                await DeleteActive(ActiveModel);

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att inaktivera odlingen!");
                return $"Gick inte att inaktivera odlingen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> DeleteActive(FarmingsActive model)
        {
            if (model == null || model.FarmingId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("DeleteActive: db == null!");

                db.FarmingsActive.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort aktiv odling.");
                return $"Gick inte att ta bort aktiv odling. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> DeleteInactive(FarmingsInactive model)
        {
            if (model == null || model.FarmingId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("DeleteInactive: db == null!");

                db.FarmingsInactive.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort inaktiv odling.");
                return $"Gick inte att ta bort inaktiv odling. Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Invalid date format: {date}");
        }

        public FarmingViewModels ChangeFromModelToViewModel(FarmingsActive model)
        {
            ArgumentNullException.ThrowIfNull(model);

            FarmingViewModels vm = _mapper.Map<FarmingViewModels>(model);

            if (!string.IsNullOrEmpty(model.PutSeedDate))
                vm.PutSeedDate = ParseDate(model.PutSeedDate);

            if (!string.IsNullOrEmpty(model.SetDate))
                vm.SetDate = ParseDate(model.SetDate);

            if (!string.IsNullOrEmpty(model.TakeUpDate))
                vm.TakeUpDate = ParseDate(model.TakeUpDate);

            return vm;
        }

        public FarmingViewModels ChangeFromModelToViewModel(FarmingsInactive model)
        {
            ArgumentNullException.ThrowIfNull(model);

            FarmingViewModels vm = _mapper.Map<FarmingViewModels>(model);

            if (!string.IsNullOrEmpty(model.PutSeedDate))
                vm.PutSeedDate = ParseDate(model.PutSeedDate);

            if (!string.IsNullOrEmpty(model.SetDate))
                vm.SetDate = ParseDate(model.SetDate);

            if (!string.IsNullOrEmpty(model.TakeUpDate))
                vm.TakeUpDate = ParseDate(model.TakeUpDate);

            return vm;
        }

        T IFarmingClass.ChangeFromViewModelToModel<T>(FarmingViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

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