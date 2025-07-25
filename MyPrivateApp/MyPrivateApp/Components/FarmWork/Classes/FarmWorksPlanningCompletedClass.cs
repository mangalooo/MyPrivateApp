
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorksPlanningCompletedClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmWorksPlanningCompletedClass> logger) : IFarmWorksPlanningCompletedClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmWorksPlanningCompletedClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Edit(FarmWorksPlanningViewModels vm)
        {
            if (vm == null || vm.FarmWorksPlanningsId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.PlanningDate == DateTime.MinValue && vm.Place != 0 && vm.Area != string.Empty)
                return "Inget datum, plats eller timmar ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                FarmWorksPlanningCompleted? model = await db.FarmWorksPlanningCompleted.FirstOrDefaultAsync(r => r.FarmWorksPlanningCompletedId == vm.FarmWorksPlanningsId);
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

        public async Task<string> Delete(FarmWorksPlanningCompleted model)
        {
            if (model == null || model.FarmWorksPlanningCompletedId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.FarmWorksPlanningCompleted.Remove(model);
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

        public FarmWorksPlanningViewModels ChangeFromModelToViewModel(FarmWorksPlanningCompleted model)
        {
            FarmWorksPlanningViewModels vm = new()
            {
                FarmWorksPlanningsId = model.FarmWorksPlanningCompletedId,
                PlanningDate = ParseDate(model.PlanningDate ?? string.Empty),
                StartDate = ParseDate(model.StartDate ?? string.Empty),
                EndDate = ParseDate(model.EndDate ?? string.Empty),
                Place = model.Place,
                Area = model.Area,
                Prioritize = model.Prioritize,
                Todo = model.Todo,
                Hectare = model.Hectare,
                Hours = model.Hours,
                Notes = model.Notes
            };

            return vm;
        }

        //public FarmWorksPlanningCompleted ChangeFromViewModelToModel(FarmWorksPlanningViewModels vm)
        //{
        //    FarmWorksPlanningCompleted model = new()
        //    {
        //        FarmWorksPlanningCompletedId = vm.FarmWorksPlanningsId,
        //        PlanningDate = vm.PlanningDate.ToString("yyyy-MM-dd"),
        //        StartDate = vm.StartDate.ToString("yyyy-MM-dd"),
        //        EndDate = vm.EndDate.ToString("yyyy-MM-dd"),
        //        Place = vm.Place,
        //        Area = vm.Area,
        //        Prioritize = vm.Prioritize,
        //        Todo = vm.Todo,
        //        Hectare = vm.Hectare,
        //        Hours = vm.Hours,
        //        Notes = vm.Notes
        //    };

        //    return model;
        //}

        private static void EditModel(FarmWorksPlanningViewModels vm, FarmWorksPlanningCompleted model)
        {
            model.FarmWorksPlanningCompletedId = vm.FarmWorksPlanningsId;
            model.PlanningDate = vm.PlanningDate.ToString("yyyy-MM-dd");
            model.StartDate = vm.StartDate.ToString("yyyy-MM-dd");
            model.EndDate = vm.EndDate.ToString("yyyy-MM-dd");
            model.Place = vm.Place;
            model.Area = vm.Area;
            model.Prioritize = vm.Prioritize;
            model.Todo = vm.Todo;
            model.Hectare = vm.Hectare;
            model.Hours = vm.Hours;
            model.Notes = vm.Notes;
        }
    }
}