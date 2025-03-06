
using AutoMapper;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorkMappingProfileClass : Profile
    {
        public FarmWorkMappingProfileClass()
        {
            CreateMap<FarmWorksViewModels, FarmWorks>();
            CreateMap<FarmWorks, FarmWorksViewModels>();
        }
    }
}