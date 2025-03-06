
using AutoMapper;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class FrozenFoodMappingProfileClass : Profile
    {
        public FrozenFoodMappingProfileClass()
        {
            CreateMap<FrozenFoodViewModel, FrozenFoods>();
            CreateMap<FrozenFoods, FrozenFoodViewModel>();
        }
    }
}