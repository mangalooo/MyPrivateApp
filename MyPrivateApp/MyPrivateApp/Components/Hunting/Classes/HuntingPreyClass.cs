
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingPreyClass : IHuntingPreyClass
    {
        private static HuntingPrey? Get(ApplicationDbContext db, int? id) => db.HuntingPrey.Any(r => r.HuntingPreyId == id) ?
                                                                                db.HuntingPrey.FirstOrDefault(r => r.HuntingPreyId == id) :
                                                                                    throw new Exception("Objektet i min jaktbytet hittades inte i databasen!");

        public string Add(ApplicationDbContext db, HuntingPreyViewModels vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        HuntingPrey model = ChangeFromViewModelToModel(vm);

                        db.HuntingPrey.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till ett nytt byte. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller typ av vilt ifyllt!";
                
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, HuntingPreyViewModels vm)
        {
            if (vm != null && vm.HuntingPreyId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        HuntingPrey getDbModel = Get(db, vm.HuntingPreyId);

                        if (getDbModel != null)
                        {
                            getDbModel.HuntingPreyId = vm.HuntingPreyId;
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.Hunter = vm.Hunter;
                            getDbModel.WildAnimal = vm.WildAnimal;
                            getDbModel.HuntingForm = vm.HuntingForm;
                            getDbModel.Type = vm.Type;
                            getDbModel.HuntingPass = vm.HuntingPass;
                            getDbModel.Dog = vm.Dog;
                            getDbModel.HuntingPlaces = vm.HuntingPlaces;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte bytet i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra bytet. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller typ av vilt ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, HuntingPreyViewModels vm, bool import)
        {
            if (vm != null && vm.HuntingPreyId > 0 && db != null)
            {
                try
                {
                    HuntingPrey model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.HuntingPrey.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort bytet. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public HuntingPreyViewModels ChangeFromModelToViewModel(HuntingPrey model)
        {
            DateTime date = DateTime.Parse(model.Date);

            HuntingPreyViewModels vm = new()
            {
                HuntingPreyId = model.HuntingPreyId,
                Date = date,
                Hunter = model.Hunter,
                WildAnimal = model.WildAnimal,
                HuntingForm = model.HuntingForm,
                Type = model.Type,
                HuntingPass = model.HuntingPass,
                Dog = model.Dog,
                HuntingPlaces = model.HuntingPlaces,
                Note = model.Note
            };

            return vm;
        }

        private static HuntingPrey ChangeFromViewModelToModel(HuntingPreyViewModels vm)
        {
            HuntingPrey huntings = new()
            {
                HuntingPreyId = vm.HuntingPreyId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Hunter = vm.Hunter,
                WildAnimal = vm.WildAnimal,
                HuntingForm = vm.HuntingForm,
                Type = vm.Type,
                HuntingPass = vm.HuntingPass,
                Dog = vm.Dog,
                HuntingPlaces = vm.HuntingPlaces,
                Note = vm.Note
            };

            return huntings;
        }
    }
}