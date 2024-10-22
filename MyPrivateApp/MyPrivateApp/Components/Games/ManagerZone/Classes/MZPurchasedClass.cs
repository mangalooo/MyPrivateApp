
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZPurchasedClass : IMZPurchasedClass
    {
        private static MZPurchasedPlayers? Get(ApplicationDbContext db, int? id) => db.MZPurchasedPlayers.Any(r => r.ManagerZonePurchasedPlayersId == id) 
                                                                                        ? db.MZPurchasedPlayers.FirstOrDefault(r => r.ManagerZonePurchasedPlayersId == id) 
                                                                                            : throw new Exception("Den köpta spelare hittades inte i databasen!");

        public string Add(ApplicationDbContext db, MZPurchasedPlayersViewModels vm)
        {
            if (vm != null && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.Salary > 0)
                {
                    try
                    {
                        MZPurchasedPlayers model = ChangeFromViewModelToModel(vm);

                        model.TrainingModeTotalCost = vm.TrainingModeCost;

                        db.MZPurchasedPlayers.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till en ny spelare. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Du måste fylla i: Köp datum, Köp värdet och Lön!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, MZPurchasedPlayersViewModels vm)
        {
            if (vm != null && vm.ManagerZonePurchasedPlayersId > 0 && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.Salary > 0)
                {
                    try
                    {
                        MZPurchasedPlayers getDbModel = Get(db, vm.ManagerZonePurchasedPlayersId);

                        if (getDbModel != null)
                        {
                            getDbModel.ManagerZonePurchasedPlayersId = vm.ManagerZonePurchasedPlayersId;
                            getDbModel.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");
                            getDbModel.Name = vm.Name;
                            getDbModel.YearsOld = vm.YearsOld;
                            getDbModel.Number = vm.Number;
                            getDbModel.PurchaseAmount = vm.PurchaseAmount;
                            getDbModel.Salary = vm.Salary;
                            getDbModel.SalarySaved = vm.SalarySaved;
                            getDbModel.TrainingModeTotalCost += vm.TrainingModeCost;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte spelaren i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra spelaren. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Du måste fylla i: Köp datum, Köp värdet och Lön!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Sell(ApplicationDbContext db, MZPurchasedPlayersViewModels vm)
        {
            int daysInTheClub = DaysInTheClub(vm.PurchasedDate);

            if (daysInTheClub < 70) return "Spelaren har inte varit i klubben i 70 dagar!";

            if (vm != null && vm.ManagerZonePurchasedPlayersId != 0 && db != null)
            {
                MZPurchasedPlayers purchasedPlayers = Get(db, vm.ManagerZonePurchasedPlayersId);

                if (purchasedPlayers == null) return "Spelaren hittades inte i databasen!";

                if (vm.SoldDate == DateTime.MinValue && vm.SoldAmount > 0)
                    return "Du måste fylla i fälten: Säljdatum och Säljvärdet!";

                if (purchasedPlayers != null)
                {
                    double totalCost = TotalCost(vm.PurchasedDate, vm.Salary, vm.PurchaseAmount, vm.TrainingModeTotalCost, vm.SaleCharge);
                    double MoneyProfitOrLoss = vm.SoldAmount - totalCost;
                    double calculateMoneyProfitOrLoss;

                    // Calculate the tax on the profit
                    if (MoneyProfitOrLoss > 0)
                    {
                        double tax = MoneyProfitOrLoss * 0.15;
                        totalCost += tax;
                        MoneyProfitOrLoss -= tax;
                        calculateMoneyProfitOrLoss = (vm.SoldAmount / totalCost) - 1;
                    }
                    else
                        calculateMoneyProfitOrLoss = (vm.SoldAmount / totalCost) - 1;

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
                        MoneyProfitOrLoss = MoneyProfitOrLoss,
                        PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss)
                    };
                    
                    try
                    {
                        db.MZSoldPlayers.Add(soldPlayer);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Lägg till såld spelare. Felmeddelande: {ex.Message} ";
                    }

                    // Removes the bought player that is moved to sold player
                    Delete(db, vm);
                }
                else
                    return "Hittar inte spelaren i databasen!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        public int DaysInTheClub(DateTime PurchasedDate)
        {
            TimeSpan DaysInTheClub = DateTime.Now - PurchasedDate;
            return DaysInTheClub.Days;
        }

        public int TotalSalary(DateTime PurchasedDate, int salary)
        {
            TimeSpan DaysInTheClub = DateTime.Now - PurchasedDate;
            int weeks = DaysInTheClub.Days / 7;
            int result = weeks * salary;
            return result;
        }

        public double TotalCost(DateTime PurchasedDate, int salary, int PurchaseAmount, int TrainingModeTotalCost, double SaleCharge)
        {
            double totalSalary = double.Parse(TotalSalary(PurchasedDate, salary).ToString());
            double result = totalSalary + PurchaseAmount + TrainingModeTotalCost + SaleCharge;
            return result;
        }

        public string Delete(ApplicationDbContext db, MZPurchasedPlayersViewModels vm)
        {
            if (vm != null && vm.ManagerZonePurchasedPlayersId > 0 && db != null)
            {
                try
                {
                    MZPurchasedPlayers model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.MZPurchasedPlayers.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort spelaren. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public MZPurchasedPlayersViewModels ChangeFromModelToViewModel(MZPurchasedPlayers model)
        {
            DateTime purchasedDate = DateTime.Parse(model.PurchasedDate);

            MZPurchasedPlayersViewModels vm = new()
            {
                ManagerZonePurchasedPlayersId = model.ManagerZonePurchasedPlayersId,
                PurchasedDate = purchasedDate,
                Name = model.Name,
                YearsOld = model.YearsOld,
                Number = model.Number,
                PurchaseAmount = model.PurchaseAmount,
                Salary = model.Salary,
                SalarySaved = model.SalarySaved,
                TrainingModeTotalCost = model.TrainingModeTotalCost,
                Note = model.Note
            };

            return vm;
        }

        private static MZPurchasedPlayers ChangeFromViewModelToModel(MZPurchasedPlayersViewModels vm)
        {
            MZPurchasedPlayers managerZoneSoldPlayers = new()
            {
                ManagerZonePurchasedPlayersId = vm.ManagerZonePurchasedPlayersId,
                PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd"),
                Name = vm.Name,
                YearsOld = vm.YearsOld,
                Number = vm.Number,
                PurchaseAmount = vm.PurchaseAmount,
                Salary = vm.Salary,
                SalarySaved = vm.SalarySaved,
                TrainingModeTotalCost = vm.TrainingModeTotalCost,
                Note = vm.Note
            };

            return managerZoneSoldPlayers;
        }
    }
}