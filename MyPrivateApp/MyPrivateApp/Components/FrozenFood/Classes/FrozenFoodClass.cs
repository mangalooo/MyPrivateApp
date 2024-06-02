using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
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
                        ErrorHandling(db, vm, "Lägg till", import, ex.Message);
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Lägg till", import, "Ingen datum eller namn ifyllt!");
                    else
                        return "Ingen datum eller namn ifyllt!";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Lägg till", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            }

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
                            getDbModel.Freezer = vm.Freezer;
                            getDbModel.FreezerCompartment = vm.FreezerCompartment;
                            getDbModel.WildMeat = vm.WildMeat;
                            getDbModel.Weight = vm.Weight;
                            getDbModel.Notes = vm.Notes;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", false, ex.Message);
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
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
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
                Freezer = model.Freezer,
                FreezerCompartment = model.FreezerCompartment,
                WildMeat = model.WildMeat,
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
                Freezer = vm.Freezer,
                FreezerCompartment = vm.FreezerCompartment,
                WildMeat = vm.WildMeat,
                Weight = vm.Weight,
                Notes = vm.Notes
            };

            return frozenFood;
        }

        private static void ErrorHandling(ApplicationDbContext db, FrozenFoodViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} Frys varor: {DateTime.Now}: Vilt: {vm.WildMeat}, Typ av vilt: {vm.Name}, Datum: {vm.Date}. "
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}