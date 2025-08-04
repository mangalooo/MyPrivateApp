
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingMyListClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<HuntingMyListClass> logger) : IHuntingMyListClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<HuntingMyListClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(HuntingMyListViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type))
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                HuntingMyList model = ChangeFromViewModelToModel(vm);

                await db.HuntingMyList.AddAsync(model);
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

        public async Task<string> Edit(HuntingMyListViewModels vm)
        {
            if (vm == null || vm.HuntingMyListId <= 0) 
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type)) 
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                HuntingMyList? model = await db.HuntingMyList.FirstOrDefaultAsync(r => r.HuntingMyListId == vm.HuntingMyListId);
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
                return $"Hittar inte bytet i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingMyList model)
        {
            if (model == null || model.HuntingMyListId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.HuntingMyList.Remove(model);
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

        public HuntingMyListViewModels ChangeFromModelToViewModel(HuntingMyList model)
        {
            HuntingMyListViewModels vm = new() 
            {
                HuntingMyListId = model.HuntingMyListId,
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

        public HuntingMyList ChangeFromViewModelToModel(HuntingMyListViewModels vm)
        {
            HuntingMyList model = new()
            {
                HuntingMyListId = vm.HuntingMyListId,
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

        private static void EditModel(HuntingMyList model, HuntingMyListViewModels vm)
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