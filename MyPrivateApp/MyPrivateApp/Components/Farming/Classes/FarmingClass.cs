
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models.Farming;

namespace MyPrivateApp.Components.Farming.Classes
{
    public class FarmingClass : IFarmingClass
    {
        private static FarmingsActive? GetActive(ApplicationDbContext db, int? id) => db.FarmingsActive.Any(r => r.FarmingId == id) ?
                                                                                        db.FarmingsActive.FirstOrDefault(r => r.FarmingId == id) :
                                                                                            throw new Exception("Odlingen hittades inte i databasen!");

        private static FarmingsInactive? GetInactive(ApplicationDbContext db, int? id) => db.FarmingsInactive.Any(r => r.FarmingId == id) ?
                                                                                        db.FarmingsInactive.FirstOrDefault(r => r.FarmingId == id) :
                                                                                            throw new Exception("Odlingen hittades inte i databasen!");


        public string Add(ApplicationDbContext db, FarmingViewModels vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (!string.IsNullOrEmpty(vm.Name) && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        FarmingsActive model = ChangeFromViewModelToModel(vm);

                        db.FarmingsActive.Add(model);
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
                        ErrorHandling(db, vm, "Lägg till", import, "Du måste fylle i namn och typ!");
                    else
                        return "Du måste fylle i namn och typ!";
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

        public string EditActive(ApplicationDbContext db, FarmingViewModels vm)
        {
            if (vm != null && vm.FarmingId > 0 && db != null)
            {
                if (!string.IsNullOrEmpty(vm.Name) && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        FarmingsActive getDbModel = GetActive(db, vm.FarmingId);

                        if (getDbModel != null)
                        {
                            getDbModel.FarmingId = vm.FarmingId;
                            getDbModel.Name = vm.Name;
                            getDbModel.Type = vm.Type;
                            getDbModel.Place = vm.Place;
                            getDbModel.PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd");
                            getDbModel.SetDate = vm.SetDate.ToString("yyyy-MM-dd");
                            getDbModel.TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd");
                            getDbModel.HowMany = vm.HowMany;
                            getDbModel.HowManySave = vm.HowManySave;
                            getDbModel.Note = vm.Note;

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
                    return "Du måste fylle i namn och typ!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string EditInactive(ApplicationDbContext db, FarmingViewModels vm)
        {
            if (vm != null && vm.FarmingId > 0 && db != null)
            {
                if (!string.IsNullOrEmpty(vm.Name) && !string.IsNullOrEmpty(vm.Type))
                {
                    try
                    {
                        FarmingsInactive getDbModel = GetInactive(db, vm.FarmingId);

                        if (getDbModel != null)
                        {
                            getDbModel.FarmingId = vm.FarmingId;
                            getDbModel.InactiveDate = vm.InactiveDate.ToString("yyyy-MM-dd");
                            getDbModel.Name = vm.Name;
                            getDbModel.Type = vm.Type;
                            getDbModel.Place = vm.Place;
                            getDbModel.PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd");
                            getDbModel.SetDate = vm.SetDate.ToString("yyyy-MM-dd");
                            getDbModel.TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd");
                            getDbModel.HowMany = vm.HowMany;
                            getDbModel.HowManySave = vm.HowManySave;
                            getDbModel.Note = vm.Note;

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
                    return "Du måste fylle i namn och typ!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Inactive(ApplicationDbContext db, FarmingViewModels vm)
        {
            if (vm != null && db != null)
            {
                FarmingsInactive farmings = new()
                {
                    InactiveDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Name = vm.Name,
                    Type = vm.Type,
                    Place = vm.Place,
                    PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd"),
                    SetDate = vm.SetDate.ToString("yyyy-MM-dd"),
                    TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd"),
                    HowMany = vm.HowMany,
                    HowManySave = vm.HowManySave,
                    Note = vm.Note
                };

                try
                {
                    db.FarmingsInactive.Add(farmings);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att inaktivera odlingen! Felmeddelande: {ex.Message}";
                }

                // Removes active farming
                DeleteActive(db, vm, false);
            }
            else
                return "Får ingen kontakt med databasen eller formuläret!";

            return string.Empty;
        }

        public string DeleteActive(ApplicationDbContext db, FarmingViewModels vm, bool import)
        {
            if (vm != null && vm.FarmingId > 0 && db != null)
            {
                try
                {
                    FarmingsActive model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.FarmingsActive.Remove(model);
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

        public string DeleteInactive(ApplicationDbContext db, FarmingViewModels vm, bool import)
        {
            if (vm != null && vm.FarmingId > 0 && db != null)
            {
                try
                {
                    FarmingsInactive model = ChangeFromViewModelToModelInactive(vm);

                    db.ChangeTracker.Clear();
                    db.FarmingsInactive.Remove(model);
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

        public FarmingViewModels ChangeFromModelToViewModel(FarmingsActive model)
        {
            DateTime putSeedDate = DateTime.Parse(model.PutSeedDate);
            DateTime setDate = DateTime.Parse(model.SetDate);
            DateTime takeUpDate = DateTime.Parse(model.TakeUpDate);

            FarmingViewModels farming = new()
            {
                FarmingId = model.FarmingId,
                Name = model.Name,
                Type = model.Type,
                Place = model.Place,
                PutSeedDate = putSeedDate,
                SetDate = setDate,
                TakeUpDate = takeUpDate,
                HowMany = model.HowMany,
                HowManySave = model.HowManySave,
                Note = model.Note
            };

            return farming;
        }

        public FarmingViewModels ChangeFromModelToViewModel(FarmingsInactive model)
        {
            DateTime putSeedDate = DateTime.Parse(model.PutSeedDate);
            DateTime setDate = DateTime.Parse(model.SetDate);
            DateTime takeUpDate = DateTime.Parse(model.TakeUpDate);
            DateTime inactiveDate = DateTime.Parse(model.InactiveDate);

            FarmingViewModels farming = new()
            {
                FarmingId = model.FarmingId,
                InactiveDate = inactiveDate,
                Name = model.Name,
                Type = model.Type,
                Place = model.Place,
                PutSeedDate = putSeedDate,
                SetDate = setDate,
                TakeUpDate = takeUpDate,
                HowMany = model.HowMany,
                HowManySave = model.HowManySave,
                Note = model.Note
            };

            return farming;
        }

        private static FarmingsActive ChangeFromViewModelToModel(FarmingViewModels vm)
        {
            FarmingsActive farmings = new()
            {
                FarmingId = vm.FarmingId,
                Name = vm.Name,
                Type = vm.Type,
                Place = vm.Place,
                PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd"),
                SetDate = vm.SetDate.ToString("yyyy-MM-dd"),
                TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd"),
                HowMany = vm.HowMany,
                HowManySave = vm.HowManySave,
                Note = vm.Note
            };

            return farmings;
        }

        private static FarmingsInactive ChangeFromViewModelToModelInactive(FarmingViewModels vm)
        {
            FarmingsInactive farmings = new()
            {
                FarmingId = vm.FarmingId,
                Name = vm.Name,
                Type = vm.Type,
                Place = vm.Place,
                PutSeedDate = vm.PutSeedDate.ToString("yyyy-MM-dd"),
                SetDate = vm.SetDate.ToString("yyyy-MM-dd"),
                TakeUpDate = vm.TakeUpDate.ToString("yyyy-MM-dd"),
                HowMany = vm.HowMany,
                HowManySave = vm.HowManySave,
                Note = vm.Note
            };

            return farmings;
        }

        private static void ErrorHandling(ApplicationDbContext db, FarmingViewModels vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} ODLING! Namn: {vm.Name} Typ: {vm.Type}. "
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}