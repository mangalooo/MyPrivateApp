﻿
using AutoMapper;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public class MZWorksMappingProfile : Profile
    {
        public MZWorksMappingProfile()
        {
            CreateMap<MZPurchasedPlayersViewModels, MZPurchasedPlayers>();
            CreateMap<MZPurchasedPlayers, MZPurchasedPlayersViewModels>();
            CreateMap<MZSoldPlayersViewModels, MZSoldPlayers>();
            CreateMap<MZSoldPlayers, MZSoldPlayersViewModels>();
        }
    }
}
