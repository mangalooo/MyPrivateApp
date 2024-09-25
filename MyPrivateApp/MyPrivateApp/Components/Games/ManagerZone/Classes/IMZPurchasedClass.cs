
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public interface IMZPurchasedClass
    {
        string Add(ApplicationDbContext db, MZPurchasedPlayersViewModels vm);
        string Edit(ApplicationDbContext db, MZPurchasedPlayersViewModels vm);
        string Sell(ApplicationDbContext db, MZPurchasedPlayersViewModels vm);
        string Delete(ApplicationDbContext db, MZPurchasedPlayersViewModels vm);
        MZPurchasedPlayersViewModels ChangeFromModelToViewModel(MZPurchasedPlayers model);
    }
}
