
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingPreyClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<HuntingPreyClass> logger) : IHuntingPreyClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<HuntingPreyClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(HuntingPreyViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type))
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                HuntingPrey model = ChangeFromViewModelToModel(vm);

                await db.HuntingPrey.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ett nytt byte!");
                return $"Gick inte att lägg till ett nytt byte! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(HuntingPreyViewModels vm)
        {
            if (vm == null || vm.HuntingPreyId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type))
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");
                
                // Fetch the entity in the same context to ensure tracking
                HuntingPrey? model = await db.HuntingPrey.FirstOrDefaultAsync(r => r.HuntingPreyId == vm.HuntingPreyId);
                if (model == null) 
                    return "Hittar inte bytet i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hittar inte bytet i databasen!");
                return $"Hittar inte jakten i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingPrey model)
        {
            if (model == null || model.HuntingPreyId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.HuntingPrey.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort bytet!");
                return $"Gick inte att ta bort bytet! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public HuntingPreyViewModels ChangeFromModelToViewModel(HuntingPrey model)
        {
            HuntingPreyViewModels vm = new()
            {
                HuntingPreyId = model.HuntingPreyId,
                Date = model.Date != null ? ParseDate(model.Date) : DateTime.MinValue,
                WildAnimal = model.WildAnimal,
                HuntingForm = model.HuntingForm,
                Type = model.Type,
                Dog = model.Dog,
                HuntingPlaces = model.HuntingPlaces,
                Note = model.Note
            };

            return vm;
        }

        public HuntingPrey ChangeFromViewModelToModel(HuntingPreyViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            HuntingPrey model = new()
            {
                HuntingPreyId = vm.HuntingPreyId,
                Date = vm.Date != DateTime.MinValue ? vm.Date.ToString("yyyy-MM-dd") : null,
                WildAnimal = vm.WildAnimal,
                HuntingForm = vm.HuntingForm,
                Type = vm.Type,
                Dog = vm.Dog,
                HuntingPlaces = vm.HuntingPlaces,
                Note = vm.Note
            };

            return model;
        }

        private static void EditModel(HuntingPrey model, HuntingPreyViewModels vm)
        {
            model.Date = vm.Date != DateTime.MinValue ? vm.Date.ToString("yyyy-MM-dd") : null;
            model.WildAnimal = vm.WildAnimal;
            model.HuntingForm = vm.HuntingForm;
            model.Type = vm.Type;
            model.Dog = vm.Dog;
            model.HuntingPlaces = vm.HuntingPlaces;
            model.Note = vm.Note;
        }
    }
}