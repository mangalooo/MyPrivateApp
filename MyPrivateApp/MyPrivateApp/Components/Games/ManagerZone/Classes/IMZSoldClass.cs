
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public interface IMZSoldClass
    {
        string Add(ApplicationDbContext db, MZSoldPlayersViewModels vm);
        string Edit(ApplicationDbContext db, MZSoldPlayersViewModels vm);
        string Delete(ApplicationDbContext db, MZSoldPlayersViewModels vm);
        MZSoldPlayersViewModels ChangeFromModelToViewModel(MZSoldPlayers model);
    }
}
