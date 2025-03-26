
using AutoMapper;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Layout.Classes
{
    public class ShopingListProfileClass : Profile
    {
        public ShopingListProfileClass()
        {
            CreateMap<ShopingListViewModels, ShopingList>();
            CreateMap<ShopingList, ShopingListViewModels>();
        }
    }
}