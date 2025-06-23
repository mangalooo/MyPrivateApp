
using AutoMapper;
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorkMappingProfileClass : Profile
    {
        public FarmWorkMappingProfileClass()
        {
            // Work hours
            CreateMap<FarmWorksViewModels, FarmWorks>();
            CreateMap<FarmWorks, FarmWorksViewModels>();

            // Planning
            CreateMap<FarmWorksPlanningViewModels, FarmWorksPlanning>();
            CreateMap<FarmWorksPlanning, FarmWorksPlanningViewModels>();
        }
    }
}