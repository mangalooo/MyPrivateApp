
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Client.ViewModels;

namespace MyPrivateApp.Components.ShoppingList.Classes
{
    public class ShopingListClass : IShopingListClass
    {
        private static ShopingList? Get(ApplicationDbContext db, int? id) => db.ShopingLists.Any(r => r.ShopingListId == id) ?
                                                                                db.ShopingLists.FirstOrDefault(r => r.ShopingListId == id) :
                                                                                    throw new Exception("Inköpslistan hittades inte i databasen!");

        public string Add(ApplicationDbContext db, ShopingListViewModels vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.List))
                {
                    try
                    {
                        ShopingList model = ChangeFromViewModelToModel(vm);

                        db.ShopingLists.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till en ny inköpslista. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller plats ifylld!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, ShopingListViewModels vm)
        {
            if (vm != null && vm.ShopingListId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.List))
                {
                    try
                    {
                        ShopingList getDbModel = Get(db, vm.ShopingListId);

                        if (getDbModel != null)
                        {
                            getDbModel.ShopingListId = vm.ShopingListId;
                            getDbModel.Name = vm.Name;
                            getDbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            getDbModel.Place = vm.Place;
                            getDbModel.List = vm.List;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra inköpslistan. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen datum eller uppgifter ifylld!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, ShopingListViewModels vm, bool import)
        {
            if (vm != null && vm.ShopingListId > 0 && db != null)
            {
                try
                {
                    ShopingList model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.ShopingLists.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort inköpslistan. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public ShopingListViewModels ChangeFromModelToViewModel(ShopingList model)
        {
            DateTime date = DateTime.Parse(model.Date);

            ShopingListViewModels shopingList = new()
            {
                ShopingListId = model.ShopingListId,
                Name = model.Name,
                Date = date,
                Place = model.Place,
                List = model.List
            };

            return shopingList;
        }

        private static ShopingList ChangeFromViewModelToModel(ShopingListViewModels vm)
        {
            ShopingList shopingList = new()
            {
                ShopingListId = vm.ShopingListId,
                Name = vm.Name,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                List = vm.List
            };

            return shopingList;
        }
    }
}