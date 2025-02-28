
using AutoMapper;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class FrozenFoodMappingProfile : Profile
    {
        public FrozenFoodMappingProfile()
        {
            CreateMap<FrozenFoodViewModel, FrozenFoods>();
            CreateMap<FrozenFoods, FrozenFoodViewModel>();
        }
    }
}