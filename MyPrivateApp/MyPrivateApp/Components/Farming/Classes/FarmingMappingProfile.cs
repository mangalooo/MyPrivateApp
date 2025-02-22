
using AutoMapper;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models.Farming;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class FarmingMappingProfile : Profile
    {
        public FarmingMappingProfile()
        {
            CreateMap<FarmingViewModels, FarmingsActive>();
            CreateMap<FarmingViewModels, FarmingsInactive>();
            CreateMap<FarmingsInactive, FarmingViewModels>();
            CreateMap<FarmingsActive, FarmingViewModels>()
                .ForMember(dest => dest.PutSeedDate, opt => opt.Ignore())
                .ForMember(dest => dest.SetDate, opt => opt.Ignore())
                .ForMember(dest => dest.TakeUpDate, opt => opt.Ignore());
        }
    }
}