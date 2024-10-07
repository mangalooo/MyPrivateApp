
//using MyPrivateApp.Data;
//using MyPrivateApp.Client.ViewModels;
//using MyPrivateApp.Data.Models;

//namespace MyPrivateApp.Components.Trip.Classes
//{
//    public class NewModulesClass : INewModulesClass
//    {
//        private static Trips? Get(ApplicationDbContext db, int? id) => db.Trips.Any(r => r.TripsId == id) ?
//                                                                                db.Trips.FirstOrDefault(r => r.TripsId == id) :
//                                                                                    throw new Exception("Resan hittades inte i databasen!");

//        public string Add(ApplicationDbContext db, TripsViewModel vm, bool import)
//        {
//            if (vm != null && db != null)
//            {
//                if (vm.Date != DateTime.MinValue && vm.HomeDate != DateTime.MinValue && !string.IsNullOrEmpty(vm.TravelBuddies))
//                {
//                    try
//                    {
//                        NewModule model = ChangeFromViewModelToModel(vm);

//                        db.Trips.Add(model);
//                        db.SaveChanges();
//                    }
//                    catch (Exception ex)
//                    {
//                        return $"Gick inte att lägg till ny resa. Felmeddelande: {ex.Message}";
//                    }
//                }
//                else
//                    return "Fyll i ut- och hem datum och vem du reste med!";
//            }
//            else
//                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

//            return string.Empty;
//        }

//        public string Edit(ApplicationDbContext db, TripsViewModel vm)
//        {
//            if (vm != null && vm.TripsId > 0 && db != null)
//            {
//                if (vm.Date != DateTime.MinValue && vm.HomeDate != DateTime.MinValue && !string.IsNullOrEmpty(vm.TravelBuddies))
//                {
//                    try
//                    {
//                        NewModule getDbModel = Get(db, vm.TripsId);

//                        if (getDbModel != null)
//                        {
//                            getDbModel.TripsId = vm.TripsId;
//                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
//                            getDbModel.Country = vm.Country;
//                            getDbModel.Description = vm.Description;
//                            getDbModel.HomeDate = vm.HomeDate.ToString("yyyy-MM-dd");
//                            getDbModel.TravelBuddies = vm.TravelBuddies;
//                            getDbModel.Place = vm.Place;
//                            getDbModel.HowManyDays = HowLongTravel(vm.Date, vm.HomeDate);

//                            db.SaveChanges();
//                        }
//                        else
//                            return "Hittar inte aktien i databasen!";
//                    }
//                    catch (Exception ex)
//                    {
//                        return $"Gick inte att ändra resan. Felmeddelande: {ex.Message}";
//                    }
//                }
//                else
//                    return "Fyll i ut- och hem datum och vem du reste med!";
//            }
//            else
//                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

//            return string.Empty;
//        }

//        public string Delete(ApplicationDbContext db, TripsViewModel vm, bool import)
//        {
//            if (vm != null && vm.TripsId > 0 && db != null)
//            {
//                try
//                {
//                    Trips model = ChangeFromViewModelToModel(vm);

//                    db.ChangeTracker.Clear();
//                    db.Trips.Remove(model);
//                    db.SaveChanges();
//                }
//                catch (Exception ex)
//                {
//                    return $"Gick inte att ta bort resan. Felmeddelande: {ex.Message}";
//                }
//            }
//            else
//                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

//            return string.Empty;
//        }

//        public TripsViewModel ChangeFromModelToViewModel(Trips model)
//        {
//            DateTime date = DateTime.Parse(model.Date);
//            DateTime homeDate = DateTime.Parse(model.HomeDate);

//            TripsViewModel trips = new()
//            {
//                TripsId = model.TripsId,
//                Date = date,
//                Country = model.Country,
//                Description = model.Description,
//                HomeDate = homeDate,
//                TravelBuddies = model.TravelBuddies,
//                Place = model.Place,
//                HowManyDays = HowLongTravel(date, homeDate)
//            };

//            return trips;
//        }

//        private static Trips ChangeFromViewModelToModel(TripsViewModel vm)
//        {
//            Trips trips = new()
//            {
//                TripsId = vm.TripsId,
//                Date = vm.Date.ToString("yyyy-MM-dd"),
//                Country = vm.Country,
//                Description = vm.Description,
//                HomeDate = vm.HomeDate.ToString("yyyy-MM-dd"),
//                TravelBuddies = vm.TravelBuddies,
//                Place = vm.Place,
//                HowManyDays = HowLongTravel(vm.Date, vm.HomeDate)
//            };

//            return trips;
//        }

//        private static double HowLongTravel(DateTime outDate, DateTime homeDate) => (homeDate - outDate).TotalDays;
//    }
//}
