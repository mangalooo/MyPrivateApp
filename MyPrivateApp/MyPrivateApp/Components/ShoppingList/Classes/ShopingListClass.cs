
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MyPrivateApp.Components.ViewModels;

namespace MyPrivateApp.Components.ShoppingList.Classes
{
    public class ShopingListClass(ApplicationDbContext db, ILogger<ShopingListClass> logger, IMapper mapper) : IShopingListClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<ShopingListClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<ShopingList?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.ShopingLists.FirstOrDefaultAsync(r => r.ShopingListId == id)
                   ?? throw new Exception("Inköpslistan hittades inte i databasen!");
        }

        public async Task<string> Add(ShopingListViewModels vm)
        {
            if (vm == null || db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.List))
                return "Ingen datum eller plats ifylld!";

            try
            {
                ShopingList model = ChangeFromViewModelToModel(vm);
                await _db.ShopingLists.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till en ny inköpslista!");
                return $"Gick inte att lägg till en ny inköpslista! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(ShopingListViewModels vm)
        {
            if (vm == null || vm.ShopingListId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue || string.IsNullOrEmpty(vm.List))
                return "Ingen datum eller uppgifter ifylld!";

            try
            {
                ShopingList? model = await Get(vm.ShopingListId);

                if (model == null)
                    return "Hittar inte inköpslistan i databasen!";

                ShopingList mapper = _mapper.Map(vm, model);
                mapper.Date = vm.Date.ToString("yyyy-MM-dd");

                await _db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra inköpslistan!");
                return "Gick inte att ändra inköpslistan!";
            }
        }

        public async Task<string> Delete(ShopingListViewModels vm)
        {
            if (vm == null || vm.ShopingListId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                ShopingList model = ChangeFromViewModelToModel(vm);
                db.ChangeTracker.Clear();
                db.ShopingLists.Remove(model);
                await db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort inköpslistan!");
                return $"Gick inte att ta bort inköpslistan! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public ShopingListViewModels ChangeFromModelToViewModel(ShopingList model)
        {
            ArgumentNullException.ThrowIfNull(model);

            ShopingListViewModels vm = _mapper.Map<ShopingListViewModels>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        private ShopingList ChangeFromViewModelToModel(ShopingListViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            ShopingList model = _mapper.Map<ShopingList>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }
    }
}