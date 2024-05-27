using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.ShoppingList.Classes
{
    public interface IShopingListClass
    {
        string Add(ApplicationDbContext db, ShopingListViewModels vm, bool import);
        string Edit(ApplicationDbContext db, ShopingListViewModels vm);
        string Delete(ApplicationDbContext db, ShopingListViewModels vm, bool import);
        ShopingListViewModels ChangeFromModelToViewModel(ShopingList model);
    }
}
