
using AutoMapper;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using MyPrivateApp.Data.Models.Hunting;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingMappingProfileClass : Profile
    {
        public HuntingMappingProfileClass()
        {
            CreateMap<HuntingMyListViewModels, HuntingMyList>();
            CreateMap<HuntingMyList, HuntingMyListViewModels>();
        }
    }
}