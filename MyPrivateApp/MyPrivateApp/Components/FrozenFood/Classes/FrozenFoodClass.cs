
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FrozenFood.Classes
{
    public class FrozenFoodClass : IFrozenFoodClass
    {
        private static FrozenFoods? Get(ApplicationDbContext db, int? id) => db.FrozenFoods.Any(r => r.FrozenFoodsId == id) ?
                                                                        db.FrozenFoods.FirstOrDefault(r => r.FrozenFoodsId == id) :
                                                                            throw new Exception("Frys varan hittades inte i databasen!");

        public string Add(ApplicationDbContext db, FrozenFoodViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Name))
                {
                    try
                    {
                        FrozenFoods model = ChangeFromViewModelToModel(vm);

                        db.FrozenFoods.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Lägg till frysvara: {vm.FrozenGoods}, Typ av vara: {vm.Name}, Datum: {vm.Date}. Felmeddelande: {ex.Message}.";
                    }
                }
                else
                    return "Ingen datum eller namn ifyllt!";
                
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, FrozenFoodViewModel vm)
        {
            if (vm != null && vm.FrozenFoodId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Name))
                {
                    try
                    {
                        FrozenFoods getDbModel = Get(db, vm.FrozenFoodId);

                        if (getDbModel != null)
                        {
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.Name = vm.Name;
                            getDbModel.Number = vm.Number;
                            getDbModel.Place = vm.Place;
                            getDbModel.FreezerCompartment = vm.FreezerCompartment;
                            getDbModel.FrozenGoods = vm.FrozenGoods;
                            getDbModel.Weight = vm.Weight;
                            getDbModel.Notes = vm.Notes;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Ändra frysvara: {vm.FrozenGoods}, Typ av vara: {vm.Name}, Datum: {vm.Date}. Felmeddelande: {ex.Message}.";
                    }
                }
                else
                    return "Ingen datum eller namn ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, FrozenFoodViewModel vm, bool import)
        {
            if (vm != null && vm.FrozenFoodId > 0 && db != null)
            {
                try
                {
                    FrozenFoods model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.FrozenFoods.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Ta bort frysvara: {vm.FrozenGoods}, Typ av vara: {vm.Name}, Datum: {vm.Date}. Felmeddelande: {ex.Message}.";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public FrozenFoodViewModel ChangeFromModelToViewModel(FrozenFoods model)
        {
            DateTime date = DateTime.Parse(model.Date);

            FrozenFoodViewModel fee = new()
            {
                FrozenFoodId = model.FrozenFoodsId,
                Name = model.Name,
                Date = date,
                Number = model.Number,
                Place = model.Place,
                FreezerCompartment = model.FreezerCompartment,
                FrozenGoods = model.FrozenGoods,
                Weight = model.Weight,
                Notes = model.Notes
            };

            return fee;
        }

        private static FrozenFoods ChangeFromViewModelToModel(FrozenFoodViewModel vm)
        {
            FrozenFoods frozenFood = new()
            {
                FrozenFoodsId = vm.FrozenFoodId,
                Name = vm.Name,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Number = vm.Number,
                Place = vm.Place,
                FreezerCompartment = vm.FreezerCompartment,
                FrozenGoods = vm.FrozenGoods,
                Weight = vm.Weight,
                Notes = vm.Notes
            };

            return frozenFood;
        }
    }
}