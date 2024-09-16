
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;

using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingTowerInspectionClass : IHuntingTowerInspectionClass
    {
        private static HuntingTowerInspection? Get(ApplicationDbContext db, int? id) => db.HuntingTowerInspections.Any(r => r.HuntingTowerInspectionId == id) ?
                                                                                            db.HuntingTowerInspections.FirstOrDefault(r => r.HuntingTowerInspectionId == id) :
                                                                                                throw new Exception("Objektet i min jaktlista hittades inte i databasen!");

        public string Add(ApplicationDbContext db, HuntingTowerInspectionViewModels vm)
        {
            if (vm != null && db != null)
            {
                if (!string.IsNullOrEmpty(vm.Number))
                {
                    try
                    {
                        HuntingTowerInspection model = ChangeFromViewModelToModel(vm);

                        db.HuntingTowerInspections.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till ett ny besikning. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Du måste fylla i ett pass nummer!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, HuntingTowerInspectionViewModels vm)
        {
            if (vm != null && vm.HuntingTowerInspectionId > 0 && db != null)
            {
                if (!string.IsNullOrEmpty(vm.Number))
                {
                    try
                    {
                        HuntingTowerInspection getDbModel = Get(db, vm.HuntingTowerInspectionId);

                        if (getDbModel != null)
                        {
                            getDbModel.HuntingTowerInspectionId = vm.HuntingTowerInspectionId;
                            getDbModel.LastInspected = vm.LastInspected.ToString("yyyy-MM-dd");
                            getDbModel.Place = vm.Place;
                            getDbModel.Inspected = vm.Inspected;
                            getDbModel.InspectedTodo = vm.InspectedTodo;
                            getDbModel.NotBeUsed = vm.NotBeUsed;
                            getDbModel.MooseTower = vm.MooseTower;
                            getDbModel.WildBoarTower = vm.WildBoarTower;
                            getDbModel.Number = vm.Number;
                            getDbModel.Todo = vm.Todo;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte besikningen i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra besikningen. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Du måste fylla i ett pass nummer!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, HuntingTowerInspectionViewModels vm)
        {
            if (vm != null && vm.HuntingTowerInspectionId > 0 && db != null)
            {
                try
                {
                    HuntingTowerInspection model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.HuntingTowerInspections.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort besikningen. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public HuntingTowerInspectionViewModels ChangeFromModelToViewModel(HuntingTowerInspection model)
        {
            DateTime date = DateTime.Parse(model.LastInspected);

            HuntingTowerInspectionViewModels vm = new()
            {
                HuntingTowerInspectionId = model.HuntingTowerInspectionId,
                LastInspected = date,
                Place = model.Place,
                Inspected = model.Inspected,
                InspectedTodo = model.InspectedTodo,
                MooseTower = model.MooseTower,
                WildBoarTower = model.WildBoarTower,
                NotBeUsed = model.NotBeUsed,
                Number = model.Number,
                Todo = model.Todo,
                Note = model.Note
            };

            return vm;
        }

        private static HuntingTowerInspection ChangeFromViewModelToModel(HuntingTowerInspectionViewModels vm)
        {
            HuntingTowerInspection inspection = new()
            {
                HuntingTowerInspectionId = vm.HuntingTowerInspectionId,
                LastInspected = vm.LastInspected.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                Inspected = vm.Inspected,
                InspectedTodo = vm.InspectedTodo,
                MooseTower = vm.MooseTower,
                WildBoarTower = vm.WildBoarTower,
                NotBeUsed = vm.NotBeUsed,
                Number = vm.Number,
                Todo = vm.Todo,
                Note = vm.Note
            };

            return inspection;
        }
    }
}