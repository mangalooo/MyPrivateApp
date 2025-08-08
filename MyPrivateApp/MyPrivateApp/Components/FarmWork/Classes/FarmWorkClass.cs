
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorkClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmWorkClass> logger) : IFarmWorksClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmWorkClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(FarmWorksViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue || vm.Place == 0 || vm.Hours == 0)
                return "Inget datum, plats eller timmar ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                FarmWorks model = ChangeFromViewModelToModel(vm);

                await db.FarmWorks.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ett nytt gårdsarbete!");
                return $"Gick inte att lägg till ett nytt gårdsarbete. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(FarmWorksViewModels vm)
        {
            if (vm == null || vm.FarmWorksId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && vm.Place != 0 && vm.Hours <= 0)
                return "Inget datum, plats eller timmar ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                FarmWorks? model = await db.FarmWorks.FirstOrDefaultAsync(r => r.FarmWorksId == vm.FarmWorksId);
                if (model == null)
                    return "Hittar inte gårdsarbetet i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra gårdsarbetet!");
                return $"Gick inte att ändra gårdsarbetet. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(FarmWorks model)
        {
            if (model == null || model.FarmWorksId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.FarmWorks.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort gårdsarbetet!");
                return $"Gick inte att ta bort gårdsarbetet. Felmeddelande: {ex.Message}";
            }
        }

        public FarmWorksViewModels ChangeFromModelToViewModel(FarmWorks model)
        {
            FarmWorksViewModels vm = new()
            {
                FarmWorksId = model.FarmWorksId,
                Date = DateTime.Parse(model.Date ?? string.Empty),
                Place = model.Place,
                PropertyDesignation = model.PropertyDesignation,
                Hours = model.Hours,
                NextSalary = model.NextSalary,
                Note = model.Note
            };

            return vm;
        }

        private static FarmWorks ChangeFromViewModelToModel(FarmWorksViewModels vm)
        {
            FarmWorks model = new()
            {
                FarmWorksId = vm.FarmWorksId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                PropertyDesignation = vm.PropertyDesignation,
                Hours = vm.Hours,
                NextSalary = vm.NextSalary,
                Note = vm.Note
            };

            return model;
        }

        private static void EditModel(FarmWorks model, FarmWorksViewModels vm)
        {
            model.FarmWorksId = vm.FarmWorksId;
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.Place = vm.Place;
            model.PropertyDesignation = vm.PropertyDesignation;
            model.Hours = vm.Hours;
            model.NextSalary = vm.NextSalary;
            model.Note = vm.Note;
        }
    }
}