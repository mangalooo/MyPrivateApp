﻿
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public interface IMZSoldClass
    {
        Task<string> Add(MZSoldPlayersViewModels vm);
        Task<string> Edit(MZSoldPlayersViewModels vm);
        Task<string> Delete(MZSoldPlayers model);
        MZSoldPlayersViewModels ChangeFromModelToViewModel(MZSoldPlayers model);
    }
}
