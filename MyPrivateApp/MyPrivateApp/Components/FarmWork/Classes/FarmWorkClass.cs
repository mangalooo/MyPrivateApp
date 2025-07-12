
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.FarmWork;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorkClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FarmWorkClass> logger, IMapper mapper) : IFarmWorksClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<FarmWorkClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

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

                model.FarmWorksId = vm.FarmWorksId;
                model.Date = vm.Date.ToString("yyyy-MM-dd");
                model.Place = vm.Place;
                model.Hours = vm.Hours;
                model.NextSalary = vm.NextSalary;
                model.Note = vm.Note;

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
            if (model == null)
                throw new Exception("ChangeFromModelToViewModel: model == null!");

            FarmWorksViewModels vm = new()
            {
                FarmWorksId = model.FarmWorksId,
                Date = DateTime.Parse(model.Date ?? throw new Exception("ChangeFromModelToViewModel: Date == null!")),
                Place = model.Place,
                Hours = model.Hours,
                NextSalary = model.NextSalary,
                Note = model.Note
            };

            return vm;
        }

        private FarmWorks ChangeFromViewModelToModel(FarmWorksViewModels vm)
        {
            if (vm == null)
                throw new Exception("ChangeFromModelToViewModel: vm == null!");

            FarmWorks model = _mapper.Map<FarmWorks>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }
    }
}