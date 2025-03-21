
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingMyListClass(ApplicationDbContext db, ILogger<HuntingMyListClass> logger, IMapper mapper) : IHuntingMyListClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<HuntingMyListClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<HuntingMyList?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.HuntingMyList.FirstOrDefaultAsync(r => r.HuntingMyListId == id)
                   ?? throw new Exception("Objektet i min jaktlista hittades inte i databasen!");
        }

        public async Task<string> Add(HuntingMyListViewModels vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type))
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                HuntingMyList model = ChangeFromViewModelToModel(vm);
                await _db.HuntingMyList.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ett nytt byte!");
                return $"Gick inte att lägg till ett nytt byte! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(HuntingMyListViewModels vm)
        {
            if (vm == null || vm.HuntingMyListId <= 0 && _db == null) 
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.Type)) 
                return "Ingen datum eller typ av vilt ifyllt!";

            try
            {
                HuntingMyList? getDbModel = await Get(vm.HuntingMyListId);

                if (getDbModel != null) return "Hittar inte bytet i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hittar inte bytet i databasen!");
                return $"Hittar inte bytet i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingMyListViewModels vm)
        {
            if (vm == null || vm.HuntingMyListId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen bytet med databasen!";

            try
            {
                HuntingMyList model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.HuntingMyList.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort bytet!");
                return $"Gick inte att ta bort bytet! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public HuntingMyListViewModels ChangeFromModelToViewModel(HuntingMyList model)
        {
            ArgumentNullException.ThrowIfNull(model);

            HuntingMyListViewModels vm = _mapper.Map<HuntingMyListViewModels>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        public HuntingMyList ChangeFromViewModelToModel(HuntingMyListViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            HuntingMyList model = _mapper.Map<HuntingMyList>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }
    }
}