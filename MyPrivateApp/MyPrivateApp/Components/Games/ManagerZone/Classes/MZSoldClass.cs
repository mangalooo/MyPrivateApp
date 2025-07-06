
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZSoldClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<MZSoldClass> logger, IMapper mapper) : IMZSoldClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<MZSoldClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<string> Add(MZSoldPlayersViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.SoldDate == DateTime.MinValue && vm.PurchaseAmount <= 0
                && vm.SalaryTotal <= 0 && vm.PurchaseAmount <= 0 && vm.TrainingModeTotalCost <= 0 && vm.SoldAmount <= 0)
                return "Datum och kostnader måste fyllas i!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                MZSoldPlayers model = ChangeFromViewModelToModel(vm);

                await db.MZSoldPlayers.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

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
            if (vm == null || vm.ManagerZoneSoldPlayerId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.SoldDate == DateTime.MinValue && vm.PurchaseAmount <= 0
                && vm.SalaryTotal <= 0 && vm.PurchaseAmount <= 0 && vm.TrainingModeTotalCost <= 0 && vm.SoldAmount <= 0)
                return "Datum och kostnader måste fyllas i!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                MZSoldPlayers? model = await db.MZSoldPlayers.FirstOrDefaultAsync(r => r.ManagerZoneSoldPlayerId == vm.ManagerZoneSoldPlayerId);
                if (model == null)
                    return "Hittar inte den sålda spelaren i databasen!";

                _mapper.Map(vm, model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra sålda spelaren!");
                return $"Gick inte att ändra sålda spelaren! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(MZSoldPlayers model)
        {
            if (model == null || model.ManagerZoneSoldPlayerId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.MZSoldPlayers.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort den sålda spelaren!");
                return $"Gick inte att ta bort den sålda spelaren! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public MZSoldPlayersViewModels ChangeFromModelToViewModel(MZSoldPlayers model)
        {
            ArgumentNullException.ThrowIfNull(model);

            MZSoldPlayersViewModels vm = _mapper.Map<MZSoldPlayersViewModels>(model);

            if (!string.IsNullOrEmpty(model.PurchasedDate))
                vm.PurchasedDate = ParseDate(model.PurchasedDate);

            if (!string.IsNullOrEmpty(model.SoldDate))
                vm.SoldDate = ParseDate(model.SoldDate);

            return vm;
        }

        public MZSoldPlayers ChangeFromViewModelToModel(MZSoldPlayersViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            MZSoldPlayers model = _mapper.Map<MZSoldPlayers>(vm);

            if (vm.PurchasedDate != DateTime.MinValue)
                model.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");

            if (vm.SoldDate != DateTime.MinValue)
                model.SoldDate = vm.SoldDate.ToString("yyyy-MM-dd");

            return model;
        }
    }
}