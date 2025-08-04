
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZPurchasedClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<MZPurchasedClass> logger) : IMZPurchasedClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<MZPurchasedClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(MZPurchasedPlayersViewModels vm)
        {
            if (vm == null )
                return "Hittar ingen data från formuläret!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.PurchaseAmount <= 0 && vm.Salary <= 0)
                return "Du måste fylla i: Köp datum, Köp värdet och Lön!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                MZPurchasedPlayers model = ChangeFromViewModelToModel(vm);

                await db.MZPurchasedPlayers.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till en ny spelare!");
                return $"Gick inte att lägg till en ny spelare. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(MZPurchasedPlayersViewModels vm)
        {
            if (vm == null || vm.ManagerZonePurchasedPlayersId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.PurchaseAmount <= 0 && vm.Salary <= 0)
                return "Du måste fylla i: Köp datum, Köp värdet och Lön!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                MZPurchasedPlayers? model = await db.MZPurchasedPlayers.FirstOrDefaultAsync(r => r.ManagerZonePurchasedPlayersId == vm.ManagerZonePurchasedPlayersId);
                if (model == null)
                    return "Hittar inte spelaren i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra spelaren!");
                return $"Gick inte att ändra spelaren. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Sell(MZPurchasedPlayersViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.ManagerZonePurchasedPlayersId == 0)
                return "ID == nulll!";

            int daysInTheClub = DaysInTheClub(vm.PurchasedDate);
            if (daysInTheClub < 70)
                return "Spelaren har inte varit i klubben i 70 dagar!";

            if (vm.SoldDate == DateTime.MinValue || vm.SoldAmount <= 0)
                return "Du måste fylla i fälten: Säljdatum och Säljvärdet!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Sell: db == null!");

                MZPurchasedPlayers? purchasedPlayer = await db.MZPurchasedPlayers.FirstOrDefaultAsync(r => r.ManagerZonePurchasedPlayersId == vm.ManagerZonePurchasedPlayersId);
                if (purchasedPlayer == null)
                    return "Spelaren hittades inte i databasen!";

                double totalCost = TotalCost(vm.PurchasedDate, vm.Salary, vm.PurchaseAmount, vm.TrainingModeTotalCost, vm.SaleCharge);
                double moneyProfitOrLoss = vm.SoldAmount - totalCost;
                double calculateMoneyProfitOrLoss = (vm.SoldAmount / totalCost) - 1;

                if (moneyProfitOrLoss > 0)
                {
                    double tax = moneyProfitOrLoss * 0.15;
                    totalCost += tax;
                    moneyProfitOrLoss -= tax;
                    calculateMoneyProfitOrLoss = (vm.SoldAmount / totalCost) - 1;
                }

                MZSoldPlayers soldPlayer = new()
                {
                    PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd"),
                    SoldDate = vm.SoldDate.ToString("yyyy-MM-dd"),
                    Name = vm.Name,
                    YearsOld = vm.YearsOld,
                    Number = vm.Number,
                    DaysInTheClub = daysInTheClub,
                    PurchaseAmount = vm.PurchaseAmount,
                    SalaryTotal = TotalSalary(vm.PurchasedDate, vm.Salary),
                    TrainingModeTotalCost = vm.TrainingModeTotalCost,
                    TotalCost = totalCost,
                    SoldAmount = vm.SoldAmount,
                    SaleCharge = vm.SaleCharge,
                    Note = vm.Note,
                    MoneyProfitOrLoss = moneyProfitOrLoss,
                    PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss)
                };

                await db.MZSoldPlayers.AddAsync(soldPlayer);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                // Removes the bought player that is moved to sold player
                MZPurchasedPlayers model = ChangeFromViewModelToModel(vm);
                await Delete(model);

                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Gick inte att lägga till såld spelare!");
                return $"Gick inte att lägga till såld spelare! Felmeddelande: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett oväntat fel inträffade!");
                return $"Ett oväntat fel inträffade. Felmeddelande: {ex.Message}";
            }
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        public int DaysInTheClub(DateTime purchasedDate) => (DateTime.Now - purchasedDate).Days;

        public int TotalSalary(DateTime purchasedDate, int salary)
        {
            int weeksInTheClub = (DateTime.Now - purchasedDate).Days / 7;
            return weeksInTheClub * salary;
        }

        public double TotalCost(DateTime purchasedDate, int salary, int purchaseAmount, int trainingModeTotalCost, double saleCharge)
        {
            int totalSalary = TotalSalary(purchasedDate, salary);
            return totalSalary + purchaseAmount + trainingModeTotalCost + saleCharge;
        }

        public async Task<string> Delete(MZPurchasedPlayers model)
        {
            if (model == null || model.ManagerZonePurchasedPlayersId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.MZPurchasedPlayers.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort spelaren!");
                return $"Gick inte att ta bort spelaren! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public MZPurchasedPlayersViewModels ChangeFromModelToViewModel(MZPurchasedPlayers model)
        {
            MZPurchasedPlayersViewModels vm = new()
            {
                ManagerZonePurchasedPlayersId = model.ManagerZonePurchasedPlayersId,
                PurchasedDate = ParseDate(model.PurchasedDate ?? string.Empty),
                PurchaseAmount = model.PurchaseAmount,
                Name = model.Name,
                YearsOld = model.YearsOld,
                Number = model.Number,
                Salary = model.Salary,
                SalarySaved = model.SalarySaved,
                Note = model.Note,
                TrainingModeTotalCost = model.TrainingModeTotalCost
            };

            return vm;
        }

        public MZPurchasedPlayers ChangeFromViewModelToModel(MZPurchasedPlayersViewModels vm)
        {
            MZPurchasedPlayers model = new()
            {
                ManagerZonePurchasedPlayersId = vm.ManagerZonePurchasedPlayersId,
                PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd"),
                PurchaseAmount = vm.PurchaseAmount,
                Name = vm.Name,
                YearsOld = vm.YearsOld,
                Number = vm.Number,
                Salary = vm.Salary,
                SalarySaved = vm.SalarySaved,
                Note = vm.Note
            };

            if (vm.TrainingModeCost != 0)
                model.TrainingModeTotalCost += vm.TrainingModeCost;

            return model;
        }

        private static void EditModel(MZPurchasedPlayers model, MZPurchasedPlayersViewModels vm)
        {
            model.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");
            model.PurchaseAmount = vm.PurchaseAmount;
            model.Name = vm.Name;
            model.YearsOld = vm.YearsOld;
            model.Number = vm.Number;
            model.Salary = vm.Salary;
            model.SalarySaved = vm.SalarySaved;
            model.Note = vm.Note;

            if (vm.TrainingModeCost != 0)
                model.TrainingModeTotalCost += vm.TrainingModeCost;
        }
    }
}