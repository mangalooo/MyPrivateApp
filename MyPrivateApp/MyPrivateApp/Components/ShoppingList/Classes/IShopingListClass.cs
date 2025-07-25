﻿
using MyPrivateApp.Data.Models;
using MyPrivateApp.Components.ViewModels;

namespace MyPrivateApp.Components.ShoppingList.Classes
{
    public interface IShopingListClass
    {
        Task<string> Add(ShopingListViewModels vm);
        Task<string> Edit(ShopingListViewModels vm);
        Task<string> Delete(ShopingList model);
        ShopingListViewModels ChangeFromModelToViewModel(ShopingList model);
    }
}