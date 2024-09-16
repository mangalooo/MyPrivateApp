
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class ManagerZonePurchasedClass : IManagerZonePurchasedClass
    {
        private static ManagerZonePurchasedPlayers? Get(ApplicationDbContext db, int? id) => db.ManagerZonePurchasedPlayers.Any(r => r.ManagerZonePurchasedPlayersId == id) ?
                                                                                                db.ManagerZonePurchasedPlayers.FirstOrDefault(r => r.ManagerZonePurchasedPlayersId == id) :
                                                                                                    throw new Exception("Objektet köpt spelare hittades inte i databasen!");

        public string Add(ApplicationDbContext db, ManagerZonePurchasedPlayersViewModels vm)
        {
            if (vm != null && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.Salary > 0)
                {
                    try
                    {
                        ManagerZonePurchasedPlayers model = ChangeFromViewModelToModel(vm);

                        db.ManagerZonePurchasedPlayers.Add(model);
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

        public string Edit(ApplicationDbContext db, ManagerZonePurchasedPlayersViewModels vm)
        {
            if (vm != null && vm.ManagerZonePurchasedPlayersId > 0 && db != null)
            {
                if (vm.PurchasedDate != DateTime.MinValue && vm.PurchaseAmount > 0 && vm.Salary > 0)
                {
                    try
                    {
                        ManagerZonePurchasedPlayers getDbModel = Get(db, vm.ManagerZonePurchasedPlayersId);

                        if (getDbModel != null)
                        {
                            getDbModel.ManagerZonePurchasedPlayersId = vm.ManagerZonePurchasedPlayersId;
                            getDbModel.PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd");
                            getDbModel.PurchaseAmount = vm.PurchaseAmount;
                            getDbModel.Salary = vm.Salary;
                            getDbModel.SalarySaved = vm.SalarySaved;
                            getDbModel.TrainingModeTotalCost = vm.TrainingModeTotalCost;
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

        public string Delete(ApplicationDbContext db, ManagerZonePurchasedPlayersViewModels vm)
        {
            if (vm != null && vm.ManagerZonePurchasedPlayersId > 0 && db != null)
            {
                try
                {
                    ManagerZonePurchasedPlayers model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.ManagerZonePurchasedPlayers.Remove(model);
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

        public ManagerZonePurchasedPlayersViewModels ChangeFromModelToViewModel(ManagerZonePurchasedPlayers model)
        {
            DateTime purchasedDate = DateTime.Parse(model.PurchasedDate);

            ManagerZonePurchasedPlayersViewModels vm = new()
            {
                ManagerZonePurchasedPlayersId = model.ManagerZonePurchasedPlayersId,
                PurchasedDate = purchasedDate,
                PurchaseAmount = model.PurchaseAmount,
                Salary = model.Salary,
                SalarySaved = model.SalarySaved,
                TrainingModeTotalCost = model.TrainingModeTotalCost,
                Note = model.Note
            };

            return vm;
        }

        private static ManagerZonePurchasedPlayers ChangeFromViewModelToModel(ManagerZonePurchasedPlayersViewModels vm)
        {
            ManagerZonePurchasedPlayers managerZoneSoldPlayers = new()
            {
                ManagerZonePurchasedPlayersId = vm.ManagerZonePurchasedPlayersId,
                PurchasedDate = vm.PurchasedDate.ToString("yyyy-MM-dd"),
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
