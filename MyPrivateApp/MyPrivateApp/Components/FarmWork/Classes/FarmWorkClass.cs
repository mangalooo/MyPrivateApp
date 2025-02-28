
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorkClass(ApplicationDbContext db, ILogger<FarmWorkClass> logger, IMapper mapper) : IFarmWorkClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<FarmWorkClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<FarmWorks?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.FarmWorks.FirstOrDefaultAsync(r => r.FarmWorksId == id)
                   ?? throw new Exception("Objektet gårdsarbete hittades inte i databasen!");
        }

        public async Task<string> Add(FarmWorksViewModels vm)
        {
            if (vm == null || _db == null) return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && vm.Hours > 0) return "Inget datum eller timmar ifyllt!";

            try
            {
                FarmWorks model = ChangeFromViewModelToModel(vm);
                await _db.FarmWorks.AddAsync(model);
                await _db.SaveChangesAsync();
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
            if (vm == null || vm.FarmWorksId <= 0 && _db == null) return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && vm.Hours > 0) return "Inget datum eller timmar ifyllt!";

            try
            {
                FarmWorks? getDbModel = await Get(vm.FarmWorksId);

                if (getDbModel == null) return "Hittar inte gårdsarbetet i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra gårdsarbetet!");
                return $"Gick inte att ändra gårdsarbetet. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(FarmWorksViewModels vm)
        {
            if (vm == null || vm.FarmWorksId <= 0 && _db == null) return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                FarmWorks model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.FarmWorks.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort gårdsarbetet!");
                return $"Gick inte att ta bort gårdsarbetet. Felmeddelande: {ex.Message}";
            }
        }

        public FarmWorksViewModels ChangeFromModelToViewModel(FarmWorks model) => _mapper.Map<FarmWorksViewModels>(model);

        private FarmWorks ChangeFromViewModelToModel(FarmWorksViewModels vm) => _mapper.Map<FarmWorks>(vm);
    }
}