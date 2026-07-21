
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZSoldClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<MZSoldClass> logger) : IMZSoldClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<MZSoldClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

                EditModel(model, vm);

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
            MZSoldPlayersViewModels vm = new()
            {
                ManagerZoneSoldPlayerId = model.ManagerZoneSoldPlayerId,
                PurchasedDate = model.PurchasedDate != null ? ParseDate(model.PurchasedDate) : DateTime.MinValue,
                SoldDate = model.SoldDate != null ? ParseDate(model.SoldDate) : DateTime.MinValue,
                PurchaseAmount = model.PurchaseAmount,
                Name = model.Name,
                YearsOld = model.YearsOld,
                Number = model.Number,
                DaysInTheClub = model.DaysInTheClub,
                SalaryTotal = model.SalaryTotal,
                TrainingModeTotalCost = model.TrainingModeTotalCost,
                TotalCost = model.TotalCost,
                SoldAmount = model.SoldAmount,
                SaleCharge = model.SaleCharge,
                MoneyProfitOrLoss = model.MoneyProfitOrLoss,
                PercentProfitOrLoss = model.PercentProfitOrLoss,
                Note = model.Note
            };

            return vm;
        }

        public MZSoldPlayers ChangeFromViewModelToModel(MZSoldPlayersViewModels vm)
        {
            MZSoldPlayers model = new()
            {
                ManagerZoneSoldPlayerId = vm.ManagerZoneSoldPlayerId,
                PurchasedDate = vm.PurchasedDate != DateTime.MinValue ? vm.PurchasedDate.ToString("yyyy-MM-dd") : null,
                SoldDate = vm.SoldDate != DateTime.MinValue ? vm.SoldDate.ToString("yyyy-MM-dd") : null,
                PurchaseAmount = vm.PurchaseAmount,
                Name = vm.Name,
                YearsOld = vm.YearsOld,
                Number = vm.Number,
                DaysInTheClub = vm.DaysInTheClub,
                SalaryTotal = vm.SalaryTotal,
                TrainingModeTotalCost = vm.TrainingModeTotalCost,
                SoldAmount = vm.SoldAmount,
                SaleCharge = (vm.SoldAmount - vm.PurchaseAmount) * 0.15,
                Note = vm.Note
            };

            model.TotalCost = vm.PurchaseAmount + vm.SalaryTotal + vm.TrainingModeTotalCost + model.SaleCharge;
            model.MoneyProfitOrLoss = vm.SoldAmount - model.TotalCost;
            model.PercentProfitOrLoss = model.TotalCost > 0
                ? $"{Math.Round((model.MoneyProfitOrLoss / model.TotalCost) * 100, 2)}%"
                : "0%"; // Avoid division by zero

            return model;
        }

        private static void EditModel(MZSoldPlayers model, MZSoldPlayersViewModels vm)
        {
            model.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");
            model.SoldDate = vm.SoldDate.ToString("yyyy-MM-dd");
            model.PurchaseAmount = vm.PurchaseAmount;
            model.Name = vm.Name;
            model.YearsOld = vm.YearsOld;
            model.Number = vm.Number;
            model.DaysInTheClub = vm.DaysInTheClub;
            model.SalaryTotal = vm.SalaryTotal;
            model.TrainingModeTotalCost = vm.TrainingModeTotalCost;
            model.SaleCharge = (vm.SoldAmount - vm.PurchaseAmount) * 0.15;
            model.TotalCost = vm.PurchaseAmount + vm.SalaryTotal + vm.TrainingModeTotalCost + model.SaleCharge;
            model.SoldAmount = vm.SoldAmount;
            model.MoneyProfitOrLoss = vm.SoldAmount - model.TotalCost;
            model.PercentProfitOrLoss = model.TotalCost > 0
                ? $"{Math.Round((model.MoneyProfitOrLoss / model.TotalCost) * 100, 2)}%"
                : "0%"; // Avoid division by zero
        }
    }
}