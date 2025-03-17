
using AutoMapper;
using MyPrivateApp.Client.ViewModels;
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