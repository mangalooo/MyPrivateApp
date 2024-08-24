
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorkClass : IFarmWorkClass
    {
        private static FarmWorks? Get(ApplicationDbContext db, int? id) => db.FarmWorks.Any(r => r.FarmWorksId == id) ?
                                                                                db.FarmWorks.FirstOrDefault(r => r.FarmWorksId == id) :
                                                                                    throw new Exception("Objektet gårdsarbete hittades inte i databasen!");

        public string Add(ApplicationDbContext db, FarmWorksViewModels vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && vm.Hours > 0)
                {
                    try
                    {
                        FarmWorks model = ChangeFromViewModelToModel(vm);

                        db.FarmWorks.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till ett nytt gårdsarbete. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Inget datum eller timmar ifyllt!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, FarmWorksViewModels vm)
        {
            if (vm != null && vm.FarmWorksId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && vm.Hours > 0)
                {
                    try
                    {
                        FarmWorks getDbModel = Get(db, vm.FarmWorksId);

                        if (getDbModel != null)
                        {
                            getDbModel.FarmWorksId = vm.FarmWorksId;
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.Place = vm.Place;
                            getDbModel.Hours = vm.Hours;
                            getDbModel.NextSalary = vm.NextSalary;
                            getDbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte jakten i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra gårdsarbetet. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Inget datum eller timmar ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, FarmWorksViewModels vm, bool import)
        {
            if (vm != null && vm.FarmWorksId > 0 && db != null)
            {
                try
                {
                    FarmWorks model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.FarmWorks.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort gårdsarbetet. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public FarmWorksViewModels ChangeFromModelToViewModel(FarmWorks model)
        {
            DateTime date = DateTime.Parse(model.Date);

            FarmWorksViewModels vm = new()
            {
                FarmWorksId = model.FarmWorksId,
                Date = date,
                Place = model.Place,
                Hours = model.Hours,
                NextSalary = model.NextSalary,
                Note = model.Note
            };

            return vm;
        }

        private static FarmWorks ChangeFromViewModelToModel(FarmWorksViewModels vm)
        {
            FarmWorks farmWorks = new()
            {
                FarmWorksId = vm.FarmWorksId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                Hours = vm.Hours,
                NextSalary = vm.NextSalary,
                Note = vm.Note
            };

            return farmWorks;
        }
    }
}