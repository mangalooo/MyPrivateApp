
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models.Hunting;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingMyListClass : IHuntingMyListClass
    {
        private static HuntingMyList? Get(ApplicationDbContext db, int? id) => db.Huntings.Any(r => r.HuntingsId == id) ?
                                                                                db.Huntings.FirstOrDefault(r => r.HuntingsId == id) :
                                                                                    throw new Exception("Jakten hittades inte i databasen!");

        public string Add(ApplicationDbContext db, HuntingViewModels vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        HuntingMyList model = ChangeFromViewModelToModel(vm);

                        db.Huntings.Add(model);
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
                        ErrorHandling(db, vm, "Lägg till", import, "Ingen datum eller typ av vilt ifyllt!");
                    else
                        return "Ingen datum eller typ av vilt ifyllt!";
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

        public string Edit(ApplicationDbContext db, HuntingViewModels vm)
        {
            if (vm != null && vm.HuntingsId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        HuntingMyList getDbModel = Get(db, vm.HuntingsId);

                        if (getDbModel != null)
                        {
                            getDbModel.HuntingsId = vm.HuntingsId;
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.WildAnimal = vm.WildAnimal;
                            getDbModel.Type = vm.Type;
                            getDbModel.Dog = vm.Dog;
                            getDbModel.HuntingPlaces = vm.HuntingPlaces;
                            getDbModel.Description = vm.Description;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte jakten i databasen!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", false, ex.Message);
                    }
                }
                else
                    return "Ingen datum eller typ av vilt ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, HuntingViewModels vm, bool import)
        {
            if (vm != null && vm.HuntingsId > 0 && db != null)
            {
                try
                {
                    HuntingMyList model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.Huntings.Remove(model);
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

        public HuntingViewModels ChangeFromModelToViewModel(HuntingMyList model)
        {
            DateTime date = DateTime.Parse(model.Date);

            HuntingViewModels vm = new()
            {
                HuntingsId = model.HuntingsId,
                Date = date,
                WildAnimal = model.WildAnimal,
                Type = model.Type,
                Dog = model.Dog,
                HuntingPlaces = model.HuntingPlaces,
                Description = model.Description
            };

            return vm;
        }

        private static HuntingMyList ChangeFromViewModelToModel(HuntingViewModels vm)
        {
            HuntingMyList huntings = new()
            {
                HuntingsId = vm.HuntingsId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                WildAnimal = vm.WildAnimal,
                Type = vm.Type,
                Dog = vm.Dog,
                HuntingPlaces = vm.HuntingPlaces,
                Description = vm.Description
            };

            return huntings;
        }

        private static void ErrorHandling(ApplicationDbContext db, HuntingViewModels vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} JAKT! Vilt: {vm.WildAnimal}, Typ: {vm.Type} Datum: {vm.Date}. "
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}