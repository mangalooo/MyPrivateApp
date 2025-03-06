
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public interface IMZSoldClass
    {
        Task<string> Add(MZSoldPlayersViewModels vm);
        Task<string> Edit(MZSoldPlayersViewModels vm);
        Task<string> Delete(MZSoldPlayersViewModels vm);
        MZSoldPlayersViewModels ChangeFromModelToViewModel(MZSoldPlayers model);
    }
}
