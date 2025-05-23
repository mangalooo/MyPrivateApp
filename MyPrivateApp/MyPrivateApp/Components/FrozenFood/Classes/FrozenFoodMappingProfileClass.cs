﻿
using AutoMapper;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FrozenFood.Classes
{
    public class FrozenMappingProfileClass : Profile
    {
        public FrozenMappingProfileClass()
        {
            CreateMap<FrozenFoodViewModel, FrozenFoods>();
            CreateMap<FrozenFoods, FrozenFoodViewModel>();
        }
    }
}