
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.FrozenFood.Classes;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZSoldClass(ApplicationDbContext db, ILogger<FrozenFoodClass> logger, IMapper mapper) : IMZSoldClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<FrozenFoodClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<MZSoldPlayers?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.MZSoldPlayers.FirstOrDefaultAsync(r => r.ManagerZoneSoldPlayerId == id)
                   ?? throw new Exception("Sålda spelaren hittades inte i databasen!");
        }

        public async Task<string> Add(MZSoldPlayersViewModels vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.SoldDate == DateTime.MinValue && vm.PurchaseAmount <= 0
                && vm.SalaryTotal <= 0 && vm.PurchaseAmount <= 0 && vm.TrainingModeTotalCost <= 0 && vm.SoldAmount <= 0)
                return "Datum och kostnader måste fyllas i!";

            try
            {
                MZSoldPlayers model = ChangeFromViewModelToModel(vm);
                await _db.MZSoldPlayers.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till en ny såld spelare!");
                return $"Gick inte att lägg till en ny såld spelare. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(MZSoldPlayersViewModels vm)
        {
            if (vm == null || vm.ManagerZoneSoldPlayerId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.SoldDate == DateTime.MinValue && vm.PurchaseAmount <= 0
                && vm.SalaryTotal <= 0 && vm.PurchaseAmount <= 0 && vm.TrainingModeTotalCost <= 0 && vm.SoldAmount <= 0)
                return "Datum och kostnader måste fyllas i!";

            try
            {
                MZSoldPlayers? getDbModel = await Get(vm.ManagerZoneSoldPlayerId);
                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra sålda spelaren!");
                return $"Gick inte att ändra sålda spelaren! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(MZSoldPlayersViewModels vm)
        {
            if (vm == null || vm.ManagerZoneSoldPlayerId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                MZSoldPlayers model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.MZSoldPlayers.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort den sålda spelaren!");
                return $"Gick inte att ta bort den sålda spelaren! Felmeddelande: {ex.Message}";
            }
        }

        public MZSoldPlayersViewModels ChangeFromModelToViewModel(MZSoldPlayers model) => _mapper.Map<MZSoldPlayersViewModels>(model);

        private MZSoldPlayers ChangeFromViewModelToModel(MZSoldPlayersViewModels vm) => _mapper.Map<MZSoldPlayers>(vm);
    }
}