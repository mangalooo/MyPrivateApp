
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZSoldClass : IMZSoldClass
    {
        private static MZSoldPlayers? Get(ApplicationDbContext db, int? id) => db.MZSoldPlayers.Any(r => r.ManagerZoneSoldPlayerId == id) ?
                                                                                            db.MZSoldPlayers.FirstOrDefault(r => r.ManagerZoneSoldPlayerId == id) :
                                                                                                throw new Exception("Objektet hittades inte i databasen!");

        public string Add(ApplicationDbContext db, MZSoldPlayersViewModels vm)
        {
            if (vm != null && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.SoldDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.SalaryTotal > 0
                    && vm.PurchaseAmount > 0 && vm.TrainingModeTotalCost > 0 && vm.SoldAmount > 0)
                {
                    try
                    {
                        MZSoldPlayers model = ChangeFromViewModelToModel(vm);

                        db.MZSoldPlayers.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till en ny spelare. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Datum och kostnader måste fyllas i!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, MZSoldPlayersViewModels vm)
        {
            if (vm != null && vm.ManagerZoneSoldPlayerId > 0 && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.SoldDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.SalaryTotal > 0
                    && vm.PurchaseAmount > 0 && vm.TrainingModeTotalCost > 0 && vm.SoldAmount > 0)
                {
                    try
                    {
                        MZSoldPlayers getDbModel = Get(db, vm.ManagerZoneSoldPlayerId);

                        if (getDbModel != null)
                        {
                            getDbModel.ManagerZoneSoldPlayerId = vm.ManagerZoneSoldPlayerId;
                            getDbModel.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");
                            getDbModel.SoldDate = vm.SoldDate.ToString("yyyy-MM-dd");
                            getDbModel.Name = vm.Name;
                            getDbModel.YearsOld = vm.YearsOld;
                            getDbModel.Number = vm.Number;
                            getDbModel.PurchaseAmount = vm.PurchaseAmount;
                            getDbModel.SalaryTotal = vm.SalaryTotal;
                            getDbModel.DaysInTheClub = vm.DaysInTheClub;
                            getDbModel.TrainingModeTotalCost = vm.TrainingModeTotalCost;
                            getDbModel.SoldAmount = vm.SoldAmount;
                            getDbModel.MoneyProfitOrLoss = vm.MoneyProfitOrLoss;
                            getDbModel.PercentProfitOrLoss = vm.PercentProfitOrLoss;
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
                    return "Datum och kostnader måste fyllas i!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, MZSoldPlayersViewModels vm)
        {
            if (vm != null && vm.ManagerZoneSoldPlayerId > 0 && db != null)
            {
                try
                {
                    MZSoldPlayers model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.MZSoldPlayers.Remove(model);
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

        public MZSoldPlayersViewModels ChangeFromModelToViewModel(MZSoldPlayers model)
        {
            DateTime purchasedDate = DateTime.Parse(model.PurchasedDate);
            DateTime soldDate = DateTime.Parse(model.SoldDate);

            MZSoldPlayersViewModels vm = new()
            {
                ManagerZoneSoldPlayerId = model.ManagerZoneSoldPlayerId,
                PurchasedDate = purchasedDate,
                SoldDate = soldDate,
                DaysInTheClub = model.DaysInTheClub,
                Name = model.Name,
                YearsOld = model.YearsOld,
                Number = model.Number,
                PurchaseAmount = model.PurchaseAmount,
                SalaryTotal = model.SalaryTotal,
                TrainingModeTotalCost = model.TrainingModeTotalCost,
                SoldAmount = model.SoldAmount,
                TotalCost = model.TotalCost,
                MoneyProfitOrLoss = model.MoneyProfitOrLoss,
                PercentProfitOrLoss = model.PercentProfitOrLoss,
                Note = model.Note
            };

            return vm;
        }

        private static MZSoldPlayers ChangeFromViewModelToModel(MZSoldPlayersViewModels vm)
        {
            MZSoldPlayers managerZoneSoldPlayers = new()
            {
                ManagerZoneSoldPlayerId = vm.ManagerZoneSoldPlayerId,
                PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd"),
                SoldDate = vm.SoldDate.ToString("yyyy-MM-dd"),
                DaysInTheClub = vm.DaysInTheClub,
                Name = vm.Name,
                YearsOld = vm.YearsOld,
                Number = vm.Number,
                PurchaseAmount = vm.PurchaseAmount,
                SalaryTotal = vm.SalaryTotal,
                TrainingModeTotalCost = vm.TrainingModeTotalCost,
                SoldAmount = vm.SoldAmount,
                TotalCost = vm.TotalCost,
                MoneyProfitOrLoss = vm.MoneyProfitOrLoss,
                PercentProfitOrLoss = vm.PercentProfitOrLoss,
                Note = vm.Note
            };

            return managerZoneSoldPlayers;
        }
    }
}
