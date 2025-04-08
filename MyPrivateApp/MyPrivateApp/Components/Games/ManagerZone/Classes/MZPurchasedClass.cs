
using AutoMapper;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Client.ViewModels;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZPurchasedClass(ApplicationDbContext db, ILogger<MZPurchasedClass> logger, IMapper mapper) : IMZPurchasedClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<MZPurchasedClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<MZPurchasedPlayers?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.MZPurchasedPlayers.FirstOrDefaultAsync(r => r.ManagerZonePurchasedPlayersId == id)
                   ?? throw new Exception("Den köpta spelare hittades inte i databasen!");
        }
        public async Task<string> Add(MZPurchasedPlayersViewModels vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.PurchaseAmount <= 0 && vm.Salary <= 0)
                return "Du måste fylla i: Köp datum, Köp värdet och Lön!";

            try
            {
                MZPurchasedPlayers model = ChangeFromViewModelToModel(vm);
                model.TrainingModeTotalCost = vm.TrainingModeCost;
                await _db.MZPurchasedPlayers.AddAsync(model);
                await _db.SaveChangesAsync();
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
            if (vm == null || vm.ManagerZonePurchasedPlayersId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.PurchasedDate == DateTime.MinValue && vm.PurchaseAmount <= 0 && vm.Salary <= 0)
                return "Du måste fylla i: Köp datum, Köp värdet och Lön!";

            try
            {
                MZPurchasedPlayers? getDbModel = await Get(vm.ManagerZonePurchasedPlayersId);

                
                if (getDbModel == null)
                    return "Hittar inte spelaren i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
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
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.ManagerZonePurchasedPlayersId == 0)
                return "Ogiltigt spelare ID!";

            int daysInTheClub = DaysInTheClub(vm.PurchasedDate);
            if (daysInTheClub < 70)
                return "Spelaren har inte varit i klubben i 70 dagar!";

            if (vm.SoldDate == DateTime.MinValue || vm.SoldAmount <= 0)
                return "Du måste fylla i fälten: Säljdatum och Säljvärdet!";

            try
            {
                MZPurchasedPlayers? purchasedPlayer = await _db.MZPurchasedPlayers
                    .FirstOrDefaultAsync(r => r.ManagerZonePurchasedPlayersId == vm.ManagerZonePurchasedPlayersId);

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

                // Removes the bought player that is moved to sold player
                await Delete(vm);

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

        public async Task<string> Delete(MZPurchasedPlayersViewModels vm)
        {
            if (vm == null || vm.ManagerZonePurchasedPlayersId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                MZPurchasedPlayers model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.MZPurchasedPlayers.Remove(model);
                await _db.SaveChangesAsync();
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
            ArgumentNullException.ThrowIfNull(model);

            MZPurchasedPlayersViewModels vm = _mapper.Map<MZPurchasedPlayersViewModels>(model);

            if (!string.IsNullOrEmpty(model.PurchasedDate))
                vm.PurchasedDate = ParseDate(model.PurchasedDate);

            return vm;
        }

        public MZPurchasedPlayers ChangeFromViewModelToModel(MZPurchasedPlayersViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            MZPurchasedPlayers model = _mapper.Map<MZPurchasedPlayers>(vm);

            if (vm.PurchasedDate != DateTime.MinValue)
                model.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");

            return model;
        }
    }
}