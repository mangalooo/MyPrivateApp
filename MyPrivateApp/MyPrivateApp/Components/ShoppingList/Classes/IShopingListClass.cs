using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.ShoppingList.Classes
{
    public interface IShopingListClass
    {
        Task<string> Add(ShopingListViewModels vm);
        Task<string> Edit(ShopingListViewModels vm);
        Task<string> Delete(ShopingListViewModels vm);
        ShopingListViewModels ChangeFromModelToViewModel(ShopingList model);
    }
}