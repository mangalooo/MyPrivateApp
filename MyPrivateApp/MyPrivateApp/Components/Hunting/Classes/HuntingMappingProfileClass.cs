
using AutoMapper;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using MyPrivateApp.Data.Models.Hunting;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingMappingProfileClass : Profile
    {
        public HuntingMappingProfileClass()
        {
            // My list
            CreateMap<HuntingMyListViewModels, HuntingMyList>();
            CreateMap<HuntingMyList, HuntingMyListViewModels>();

            // Prey
            CreateMap<HuntingPrey, HuntingPreyViewModels>();
            CreateMap<HuntingPreyViewModels, HuntingPrey>();

            // Team members
            CreateMap<HuntingTeamMembers, HuntingTeamMembersViewModels>();
            CreateMap<HuntingTeamMembersViewModels, HuntingTeamMembers>();

            // Tower inspections
            CreateMap<HuntingTowerInspection, HuntingTowerInspectionViewModels>();
            CreateMap<HuntingTowerInspectionViewModels, HuntingTowerInspection>();
        }
    }
}