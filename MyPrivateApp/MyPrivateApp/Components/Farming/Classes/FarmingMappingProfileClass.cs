
using AutoMapper;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models.Farming;

namespace MyPrivateApp.Components.Farming.Classes
{
    public class FarmingMappingProfileClass : Profile
    {
        public FarmingMappingProfileClass()
        {
            //Inactive 
            CreateMap<FarmingViewModels, FarmingsInactive>();
            CreateMap<FarmingsInactive, FarmingViewModels>();

            // Active
            CreateMap<FarmingViewModels, FarmingsActive>();
            CreateMap<FarmingsActive, FarmingViewModels>()
                .ForMember(dest => dest.PutSeedDate, opt => opt.Ignore())
                .ForMember(dest => dest.SetDate, opt => opt.Ignore())
                .ForMember(dest => dest.TakeUpDate, opt => opt.Ignore());
        }
    }
}