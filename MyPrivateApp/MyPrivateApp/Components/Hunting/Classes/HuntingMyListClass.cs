
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models.Hunting;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingMyListClass : IHuntingMyListClass
    {
        private static HuntingMyList? Get(ApplicationDbContext db, int? id) => db.HuntingMyList.Any(r => r.HuntingMyListId == id) ?
                                                                                db.HuntingMyList.FirstOrDefault(r => r.HuntingMyListId == id) :
                                                                                    throw new Exception("Objektet i min jaktlista hittades inte i databasen!");

        public string Add(ApplicationDbContext db, HuntingMyListViewModels vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        HuntingMyList model = ChangeFromViewModelToModel(vm);

                        db.HuntingMyList.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Lägg till ett objekt ur min lista! Vilt: {vm.WildAnimal}, Typ: {vm.Type} Datum: {vm.Date}. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller typ av vilt ifyllt!";
                
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, HuntingMyListViewModels vm)
        {
            if (vm != null && vm.HuntingMyListId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        HuntingMyList getDbModel = Get(db, vm.HuntingMyListId);

                        if (getDbModel != null)
                        {
                            getDbModel.HuntingMyListId = vm.HuntingMyListId;
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.WildAnimal = vm.WildAnimal;
                            getDbModel.Type = vm.Type;
                            getDbModel.Dog = vm.Dog;
                            getDbModel.HuntingPlaces = vm.HuntingPlaces;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte jakten i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Ändra ett objekt ur min lista! Vilt: {vm.WildAnimal}, Typ: {vm.Type} Datum: {vm.Date}. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller typ av vilt ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, HuntingMyListViewModels vm, bool import)
        {
            if (vm != null && vm.HuntingMyListId > 0 && db != null)
            {
                try
                {
                    HuntingMyList model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.HuntingMyList.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Ta bort ett objekt ur min lista! Vilt: {vm.WildAnimal}, Typ: {vm.Type} Datum: {vm.Date}. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public HuntingMyListViewModels ChangeFromModelToViewModel(HuntingMyList model)
        {
            DateTime date = DateTime.Parse(model.Date);

            HuntingMyListViewModels vm = new()
            {
                HuntingMyListId = model.HuntingMyListId,
                Date = date,
                WildAnimal = model.WildAnimal,
                Type = model.Type,
                Dog = model.Dog,
                HuntingPlaces = model.HuntingPlaces,
                Note = model.Note
            };

            return vm;
        }

        private static HuntingMyList ChangeFromViewModelToModel(HuntingMyListViewModels vm)
        {
            HuntingMyList huntings = new()
            {
                HuntingMyListId = vm.HuntingMyListId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                WildAnimal = vm.WildAnimal,
                Type = vm.Type,
                Dog = vm.Dog,
                HuntingPlaces = vm.HuntingPlaces,
                Note = vm.Note
            };

            return huntings;
        }
    }
}