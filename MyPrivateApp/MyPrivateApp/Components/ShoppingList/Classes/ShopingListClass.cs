using MyPrivateApp.Data.Models.SharesModels;
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
                if (vm.Date != DateTime.MinValue && string.IsNullOrEmpty(vm.List))
                {
                    try
                    {
                        ShopingList model = ChangeFromViewModelToModel(vm);

                        db.ShopingLists.Add(model);
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
                        ErrorHandling(db, vm, "Lägg till", import, "Ingen datum ifyllt eller plats är i fyllt!");
                    else
                        return "Ingen datum ifyllt eller plats är i fyllt!";
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

        public string Edit(ApplicationDbContext db, ShopingListViewModels vm)
        {
            if (vm != null && vm.ShopingListId > 0 && db != null)
            {
                if (vm.Date != DateTime.MinValue && string.IsNullOrEmpty(vm.List))
                {
                    try
                    {
                        ShopingList getDbModel = Get(db, vm.ShopingListId);

                        if (getDbModel != null)
                        {
                            getDbModel.ShopingListId = vm.ShopingListId;
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
                        ErrorHandling(db, vm, "Ändra", false, ex.Message);
                    }
                }
                else
                    return "Ingen datum ifyllt eller någon av skatt eller courtage måste vara mer än 0!";
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
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
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
                Date = vm.Date.ToString("yyyy-MM-dd"),
                Place = vm.Place,
                List = vm.List
            };

            return shopingList;
        }

        private static void ErrorHandling(ApplicationDbContext db, ShopingListViewModels vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} inköpslista: {DateTime.Now}: Plats: {vm.Place}, Listan: {vm.List}, Datum: {vm.Date}. "
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}