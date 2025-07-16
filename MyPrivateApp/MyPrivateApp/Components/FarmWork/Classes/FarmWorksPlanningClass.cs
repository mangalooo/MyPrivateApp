
using MyPrivateApp.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorksPlanningClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmWorksPlanningClass> logger, IMapper mapper) : IFarmWorksPlanningClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmWorksPlanningClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<string> Add(FarmWorksPlanningViewModels vm)
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null)
                    return "Hittar ingen data från formuläret!";

                if (vm.Date == DateTime.MinValue || vm.Place == 0 || vm.Area != string.Empty)
                    return "Inget datum, plats eller timmar ifyllt!";

                FarmWorksPlanning model = ChangeFromViewModelToModel(vm);

                await db.FarmWorksPlanning.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ett nytt skogspanneringen!");
                return $"Gick inte att lägg till ett nytt skogspanneringen. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(FarmWorksPlanningViewModels vm)
        {
            if (vm == null || vm.FarmWorksId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && vm.Place != 0 && vm.Area != string.Empty)
                return "Inget datum, plats eller timmar ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                FarmWorksPlanning? model = await db.FarmWorksPlanning.FirstOrDefaultAsync(r => r.FarmWorksId == vm.FarmWorksId);
                if (model == null)
                    return "Hittar inte skogspanneringen i databasen!";

                _mapper.Map(vm, model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra skogspanneringen!");
                return $"Gick inte att ändra skogspanneringen. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(FarmWorksPlanning model)
        {
            if (model == null || model.FarmWorksId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.FarmWorksPlanning.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort skogspanneringen!");
                return $"Gick inte att ta bort skogspanneringen. Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public FarmWorksPlanningViewModels ChangeFromModelToViewModel(FarmWorksPlanning model)
        {
            ArgumentNullException.ThrowIfNull(model);

            FarmWorksPlanningViewModels vm = _mapper.Map<FarmWorksPlanningViewModels>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        public FarmWorksPlanning ChangeFromViewModelToModel(FarmWorksPlanningViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            FarmWorksPlanning model = _mapper.Map<FarmWorksPlanning>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }
    }
}