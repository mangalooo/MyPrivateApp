
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Farming;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels;

namespace MyPrivateApp.Components.Farming.Classes
{
    public class FarmingClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmingClass> logger) : IFarmingClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmingClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(FarmingViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (string.IsNullOrEmpty(vm.Name) || string.IsNullOrEmpty(vm.Type))
                return "Du måste fylle i namn och typ!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                FarmingsActive model = ChangeFromViewModelToModelActive(vm);

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

                EditModelActive(model, vm);

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
                FarmingsInactive? model = await db.FarmingsInactive.FirstOrDefaultAsync(r => r.FarmingId == vm.FarmingId);

                if (model == null)
                    return "Hittar inte odlingen i databasen!";

                EditModelInactive(model,  vm);

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

                FarmingsInactive model = ChangeFromViewModelToModelInactive(vm);
                model.InactiveDate = DateTime.Now.ToString("yyyy-MM-dd");

                await db.FarmingsInactive.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                // Removes active farming
                FarmingsActive ActiveModel = ChangeFromViewModelToModelActive(vm);
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
            FarmingViewModels vm = new()
            {
                FarmingId = model.FarmingId,
                Name = model.Name,
                Type = model.Type,
                Place = model.Place,
                PutSeedDate = ParseDate(model.PutSeedDate ?? string.Empty),
                SetDate = ParseDate(model.SetDate ?? string.Empty),
                TakeUpDate = ParseDate(model.TakeUpDate ?? string.Empty),
                HowMany = model.HowMany,
                HowManySave = model.HowManySave,
                Note = model.Note,
            };

            return vm;
        }

        public FarmingViewModels ChangeFromModelToViewModel(FarmingsInactive model)
        {
            FarmingViewModels vm = new()
            {
                FarmingId = model.FarmingId,
                Name = model.Name,
                Type = model.Type,
                Place = model.Place,
                InactiveDate = ParseDate(model.InactiveDate ?? string.Empty),
                PutSeedDate = ParseDate(model.PutSeedDate ?? string.Empty),
                SetDate = ParseDate(model.SetDate ?? string.Empty),
                TakeUpDate = ParseDate(model.TakeUpDate ?? string.Empty),
                HowMany = model.HowMany,
                HowManySave = model.HowManySave,
                Note = model.Note
            };

            return vm;
        }

        public FarmingsActive ChangeFromViewModelToModelActive(FarmingViewModels vm)
        {
            FarmingsActive model = new()
            {
                FarmingId = vm.FarmingId,
                Name = vm.Name,
                Type = vm.Type,
                Place = vm.Place,
                PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd"),
                SetDate = vm.SetDate.ToString("yyyy-MM-dd"),
                TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd"),
                HowMany = vm.HowMany,
                HowManySave = vm.HowManySave,
                Note = vm.Note
            };

            return model;
        }

        public FarmingsInactive ChangeFromViewModelToModelInactive(FarmingViewModels vm)
        {
            FarmingsInactive model = new()
            {
                Name = vm.Name,
                Type = vm.Type,
                Place = vm.Place,
                PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd"),
                SetDate = vm.SetDate.ToString("yyyy-MM-dd"),
                TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd"),
                HowMany = vm.HowMany,
                HowManySave = vm.HowManySave,
                Note = vm.Note,
                InactiveDate = vm.InactiveDate.ToString("yyyy-MM-dd"),
            };

            return model;
        }

        private static void EditModelActive(FarmingsActive model, FarmingViewModels vm)
        {
            model.Name = vm.Name;
            model.Type = vm.Type;
            model.Place = vm.Place;
            model.PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd");
            model.SetDate = vm.SetDate.ToString("yyyy-MM-dd");
            model.TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd");
            model.HowMany = vm.HowMany;
            model.HowManySave = vm.HowManySave;
            model.Note = vm.Note;
        }

        private static void EditModelInactive(FarmingsInactive model, FarmingViewModels vm)
        {
            model.Name = vm.Name;
            model.Type = vm.Type;
            model.Place = vm.Place;
            model.PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd");
            model.SetDate = vm.SetDate.ToString("yyyy-MM-dd");
            model.TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd");
            model.HowMany = vm.HowMany;
            model.HowManySave = vm.HowManySave;
            model.Note = vm.Note;
            model.InactiveDate = vm.InactiveDate.ToString("yyyy-MM-dd");
        }
    }
}