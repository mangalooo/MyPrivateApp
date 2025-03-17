
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Trip.Classes
{
    public class TripClass(ApplicationDbContext db, ILogger<TripClass> logger, IMapper mapper) : ITripClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<TripClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<Trips?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.Trips.FirstOrDefaultAsync(r => r.TripsId == id)
                   ?? throw new Exception("Resan hittades inte i databasen!");
        }

        public async Task<string> Add(TripsViewModel vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue || vm.HomeDate == DateTime.MinValue || string.IsNullOrEmpty(vm.TravelBuddies))
                return "Fyll i ut- och hem datum och vem du reste med!";

            try
            {
                Trips model = ChangeFromViewModelToModel(vm);
                await _db.Trips.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ny resa!");
                return "Gick inte att lägg till ny resa!";
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
                Trips? getDbModel = await Get(vm.TripsId);

                if (getDbModel == null)
                    return "Hittar inte resan i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra resan!");
                return "Gick inte att ändra resan!";
            }
        }

        public async Task<string> Delete(TripsViewModel vm)
        {
            if (vm == null || vm.TripsId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                Trips model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.Trips.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort resan!");
                return "Gick inte att ta bort resan!";
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
            ArgumentNullException.ThrowIfNull(model);

            TripsViewModel vm = _mapper.Map<TripsViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            if (!string.IsNullOrEmpty(model.HomeDate))
                vm.HomeDate = ParseDate(model.HomeDate);

            return vm;
        }

        public Trips ChangeFromViewModelToModel(TripsViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            Trips model = _mapper.Map<Trips>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            if (vm.HomeDate != DateTime.MinValue)
                model.HomeDate = vm.HomeDate.ToString("yyyy-MM-dd");

            return model;
        }

        private static double HowLongTravel(DateTime outDate, DateTime homeDate) => (homeDate - outDate).TotalDays;
    }
}