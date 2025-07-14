
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.ShoppingList.Classes
{
    public class ShopingListClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<ShopingListClass> logger) : IShopingListClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<ShopingListClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(ShopingListViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Date == DateTime.MinValue && string.IsNullOrEmpty(vm.List))
                return "Ingen datum eller plats ifylld!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                ShopingList model = ChangeFromViewModelToModel(vm);
                
                await db.ShopingLists.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

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
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                ShopingList? model = await db.ShopingLists.FirstOrDefaultAsync(r => r.ShopingListId == vm.ShopingListId);

                if (model == null)
                    return "Hittar inte inköpslistan i databasen!";

                EditModel(vm, model);

                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra inköpslistan!");
                return "Gick inte att ändra inköpslistan!";
            }
        }

        public async Task<string> Delete(ShopingList model)
        {
            if (model == null || model.ShopingListId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.ShopingLists.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
                
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
            ShopingListViewModels vm = new()
            {
                ShopingListId = model.ShopingListId,
                Name = model.Name,
                Date = ParseDate(model.Date ?? throw new Exception("ChangeFromModelToViewModel: Date == null!")),
                Place = model.Place,
                List = model.List
            };

            return vm;
        }
        
        private static ShopingList ChangeFromViewModelToModel(ShopingListViewModels vm)
        {
            ShopingList model = new()
            {
                ShopingListId = vm.ShopingListId,
                Name = vm.Name,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                List = vm.List
            };

            return model;
        }

        private static void EditModel(ShopingListViewModels vm, ShopingList model)
        {
            model.ShopingListId = vm.ShopingListId;
            model.Name = vm.Name;
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.Place = vm.Place;
            model.List = vm.List;
        }
    }
}