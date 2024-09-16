
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public interface IManagerZonePurchasedClass
    {
        string Add(ApplicationDbContext db, ManagerZonePurchasedPlayersViewModels vm);
        string Edit(ApplicationDbContext db, ManagerZonePurchasedPlayersViewModels vm);
        string Delete(ApplicationDbContext db, ManagerZonePurchasedPlayersViewModels vm);
        ManagerZonePurchasedPlayersViewModels ChangeFromModelToViewModel(ManagerZonePurchasedPlayers model);
    }
}
