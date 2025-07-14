
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Trip.Classes
{
    public class TripClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<TripClass> logger) : ITripClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<TripClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(TripsViewModel vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue || vm.HomeDate == DateTime.MinValue || string.IsNullOrEmpty(vm.TravelBuddies))
                return "Fyll i ut- och hem datum och vem du reste med!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                Trips model = ChangeFromViewModelToModel(vm);

                await db.Trips.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ny resa!");
                return $"Gick inte att lägg till ny resa. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(TripsViewModel vm)
        {
            if (vm == null || vm.TripsId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue || vm.HomeDate == DateTime.MinValue || string.IsNullOrEmpty(vm.TravelBuddies))
                return "Fyll i ut- och hem datum och vem du reste med!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                Trips? model = await db.Trips.FirstOrDefaultAsync(r => r.TripsId == vm.TripsId);

                if (model == null)
                    return "Hittar inte resan i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra resan!");
                return $"Gick inte att ändra resan! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(Trips model)
        {
            if (model == null || model.TripsId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.Trips.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort resan!");
                return $"Gick inte att ta bort resan! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public TripsViewModel ChangeFromModelToViewModel(Trips model)
        {
            DateTime date = ParseDate(model.Date ?? throw new Exception("ChangeFromModelToViewModel: Date == null!"));
            DateTime homeDate = ParseDate(model.HomeDate ?? throw new Exception("ChangeFromModelToViewModel: Date == null!"));

            TripsViewModel vm = new()
            {
                TripsId = model.TripsId,
                Country = model.Country,
                Place = model.Place,
                Date = date,
                HomeDate = homeDate,
                HowManyDays = HowLongTravel(date, homeDate),
                TravelBuddies = model.TravelBuddies,
                Description = model.Description
            };

            return vm;
        }

        private static Trips ChangeFromViewModelToModel(TripsViewModel vm)
        {
            Trips model = new()
            {
                TripsId = vm.TripsId,
                Country = vm.Country,
                Place = vm.Place,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                HomeDate = vm.HomeDate.ToString("yyyy-MM-dd"),
                HowManyDays = HowLongTravel(vm.Date, vm.HomeDate),
                TravelBuddies = vm.TravelBuddies,
                Description = vm.Description
            };

            return model;
        }

        private static void EditModel(Trips model, TripsViewModel vm)
        {
            model.TripsId = vm.TripsId;
            model.Country = vm.Country;
            model.Place = vm.Place;
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.HomeDate = vm.HomeDate.ToString("yyyy-MM-dd");
            model.HowManyDays = HowLongTravel(vm.Date, vm.HomeDate);
            model.TravelBuddies = vm.TravelBuddies;
            model.Description = vm.Description;
        }

        private static double HowLongTravel(DateTime outDate, DateTime homeDate) => (homeDate - outDate).TotalDays;
    }
}