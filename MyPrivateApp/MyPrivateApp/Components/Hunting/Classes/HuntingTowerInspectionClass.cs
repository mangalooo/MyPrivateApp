
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingTowerInspectionClass(ApplicationDbContext db, ILogger<HuntingTowerInspectionClass> logger, IMapper mapper) : IHuntingTowerInspectionClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<HuntingTowerInspectionClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<HuntingTowerInspection?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.HuntingTowerInspections.FirstOrDefaultAsync(r => r.HuntingTowerInspectionId == id)
                   ?? throw new Exception("Objektet i min jaktlista hittades inte i databasen!");
        }

        public async Task<string> Add(HuntingTowerInspectionViewModels vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (string.IsNullOrEmpty(vm.Number))
                return "Du måste fylla i ett pass nummer!";

            try
            {
                HuntingTowerInspection model = ChangeFromViewModelToModel(vm);
                await _db.HuntingTowerInspections.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ett ny besikning!");
                return $"Gick inte att lägg till ett ny besikning! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(HuntingTowerInspectionViewModels vm)
        {
            if (vm == null || vm.HuntingTowerInspectionId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (string.IsNullOrEmpty(vm.Number))
                return "Du måste fylla i ett pass nummer!";

            try
            {
                HuntingTowerInspection? getDbModel = await Get(vm.HuntingTowerInspectionId);

                if (getDbModel != null) return "Hittar inte bytet i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hittar inte besikningen i databasen!");
                return $"Hittar inte besikningen i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingTowerInspectionViewModels vm)
        {
            if (vm == null || vm.HuntingTowerInspectionId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen besikningen med databasen!";

            try
            {
                HuntingTowerInspection model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.HuntingTowerInspections.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort besikningen!");
                return $"Gick inte att ta bort besikningen! Felmeddelande: {ex.Message}";
            }
        }
        public HuntingTowerInspectionViewModels ChangeFromModelToViewModel(HuntingTowerInspection model) => _mapper.Map<HuntingTowerInspectionViewModels>(model);

        private HuntingTowerInspection ChangeFromViewModelToModel(HuntingTowerInspectionViewModels vm) => _mapper.Map<HuntingTowerInspection>(vm);
    }
}