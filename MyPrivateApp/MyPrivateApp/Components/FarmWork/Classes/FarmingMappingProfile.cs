
using AutoMapper;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class FarmWorksMappingProfile : Profile
    {
        public FarmWorksMappingProfile()
        {
            CreateMap<FarmWorksViewModels, FarmWorks>();
            CreateMap<FarmWorks, FarmWorksViewModels>();
        }
    }
}