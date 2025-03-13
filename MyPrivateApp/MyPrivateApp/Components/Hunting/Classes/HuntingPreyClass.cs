
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using AutoMapper;
using MyPrivateApp.Components.FrozenFood.Classes;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingPreyClass(ApplicationDbContext db, ILogger<HuntingPreyClass> logger, IMapper mapper) : IHuntingPreyClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<HuntingPreyClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<HuntingPrey?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.HuntingPrey.FirstOrDefaultAsync(r => r.HuntingPreyId == id)
                   ?? throw new Exception("Objektet i min jaktbytet hittades inte i databasen!");
        }

        public async Task<string> Add(HuntingPreyViewModels vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type))
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                HuntingPrey model = ChangeFromViewModelToModel(vm);
                await _db.HuntingPrey.AddAsync(model);
                await _db.SaveChangesAsync();
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
            if (vm == null || vm.HuntingPreyId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type))
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                HuntingPrey? getDbModel = await Get(vm.HuntingPreyId);

                if (getDbModel != null) return "Hittar inte bytet i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hittar inte bytet i databasen!");
                return $"Hittar inte jakten i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingPreyViewModels vm)
        {
            if (vm == null || vm.HuntingPreyId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                HuntingPrey model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.HuntingPrey.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort bytet!");
                return $"Gick inte att ta bort bytet! Felmeddelande: {ex.Message}";
            }
        }

        public HuntingPreyViewModels ChangeFromModelToViewModel(HuntingPrey model) => _mapper.Map<HuntingPreyViewModels>(model);

        private HuntingPrey ChangeFromViewModelToModel(HuntingPreyViewModels vm) => _mapper.Map<HuntingPrey>(vm);
    }
}