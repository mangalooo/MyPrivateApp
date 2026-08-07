
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorksPlanningClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmWorksPlanningClass> logger) : IFarmWorksPlanningClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmWorksPlanningClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(FarmWorksPlanningViewModels vm)
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Add: db == null!");

                if (vm == null)
                    return "Hittar ingen data från formuläret!";

                if (vm.Place == 0 || vm.Area == string.Empty)
                    return "Plats, område eller timmar ifyllt!";

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
            if (vm == null || vm.FarmWorksPlanningsId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Place == 0 || vm.Area == string.Empty)
                return "Plats, område eller timmar ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                FarmWorksPlanning? model = await db.FarmWorksPlanning.FirstOrDefaultAsync(r => r.FarmWorksPlanningsId == vm.FarmWorksPlanningsId);
                if (model == null)
                    return "Hittar inte skogspanneringen i databasen!";

                EditModel(vm, model);

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

        public async Task<string> Completed(FarmWorksPlanningViewModels vm)
        {
            if (vm == null || vm.FarmWorksPlanningsId <= 0)
                return "Får ingen kontakt med formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Completed: db == null!");

                await using var transaction = await db.Database.BeginTransactionAsync();

                FarmWorksPlanning? planning = await db.FarmWorksPlanning
                    .Include(x => x.FarmWorks)
                    .FirstOrDefaultAsync(x => x.FarmWorksPlanningsId == vm.FarmWorksPlanningsId);

                if (planning == null)
                    return "Hittar inte skogsplaneringen i databasen!";

                FarmWorksPlanningCompleted modelCompleted = ChangeFromViewModelToModelCompleted(vm);
                modelCompleted.EndDate = DateTime.Now.ToString("yyyy-MM-dd");

                await db.FarmWorksPlanningCompleted.AddAsync(modelCompleted);
                await db.SaveChangesAsync();

                foreach (FarmWorks farmWork in planning.FarmWorks)
                {
                    farmWork.FarmWorksPlanningsId = null;
                    farmWork.FarmWorksPlanningCompletedId = modelCompleted.FarmWorksPlanningCompletedId;
                }

                db.FarmWorksPlanning.Remove(planning);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                db.ChangeTracker.Clear();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att inaktivera skogsplaneringen!");
                return $"Gick inte att inaktivera skogsplaneringen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(FarmWorksPlanning model)
        {
            if (model == null || model.FarmWorksPlanningsId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Delete: db == null!");

                bool hasRelatedFarmWorks = await db.FarmWorks
                    .AnyAsync(x => x.FarmWorksPlanningsId == model.FarmWorksPlanningsId);

                if (hasRelatedFarmWorks)
                {
                    return "Går inte att ta bort skogsplaneringen eftersom det finns relaterade gårdsarbeten kopplade till den.";
                }

                db.FarmWorksPlanning.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();

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
            FarmWorksPlanningViewModels vm = new()
            {
                FarmWorksPlanningsId = model.FarmWorksPlanningsId,
                StartDate = ParseDate(model.StartDate ?? string.Empty),
                Place = model.Place,
                Area = model.Area,
                PropertyDesignation = model.PropertyDesignation,
                Prioritize = model.Prioritize,
                Hectare = model.Hectare,
                Hours = model.Hours,
                ClearingHours = model.ClearingHours,
                ForClearingHours = model.ForClearingHours,
                ThinHours = model.ThinHours,
                ToForwardHours = model.ToForwardHours,
                Notes = model.Notes
            };

            return vm;
        }

        public FarmWorksPlanning ChangeFromViewModelToModel(FarmWorksPlanningViewModels vm)
        {
            FarmWorksPlanning model = new()
            {
                FarmWorksPlanningsId = vm.FarmWorksPlanningsId,
                StartDate = vm.StartDate.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                Area = vm.Area,
                PropertyDesignation = vm.PropertyDesignation,
                Prioritize = vm.Prioritize,
                Hectare = vm.Hectare,
                Hours = vm.Hours,
                ClearingHours = vm.ClearingHours,
                ForClearingHours = vm.ForClearingHours,
                ThinHours = vm.ThinHours,
                ToForwardHours = vm.ToForwardHours,
                Notes = vm.Notes
            };

            return model;
        }

        private static void EditModel(FarmWorksPlanningViewModels vm, FarmWorksPlanning model)
        {
            model.FarmWorksPlanningsId = vm.FarmWorksPlanningsId;
            model.StartDate = vm.StartDate.ToString("yyyy-MM-dd");
            model.Place = vm.Place;
            model.Area = vm.Area;
            model.PropertyDesignation = vm.PropertyDesignation;
            model.Prioritize = vm.Prioritize;
            model.Hectare = vm.Hectare;
            model.Hours = vm.Hours;
            model.ClearingHours = vm.ClearingHours;
            model.ForClearingHours = vm.ForClearingHours;
            model.ThinHours = vm.ThinHours;
            model.ToForwardHours = vm.ToForwardHours;
            model.Notes = vm.Notes;
        }

        private static FarmWorksPlanningCompleted ChangeFromViewModelToModelCompleted(FarmWorksPlanningViewModels vm)
        {
            FarmWorksPlanningCompleted model = new()
            {
                StartDate = vm.StartDate.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                Area = vm.Area,
                PropertyDesignation = vm.PropertyDesignation,
                Prioritize = vm.Prioritize,
                Hectare = vm.Hectare,
                Hours = vm.Hours,
                ClearingHours = vm.ClearingHours,
                ForClearingHours = vm.ForClearingHours,
                ThinHours = vm.ThinHours,
                ToForwardHours = vm.ToForwardHours,
                Notes = vm.Notes
            };

            return model;
        }
    }
}