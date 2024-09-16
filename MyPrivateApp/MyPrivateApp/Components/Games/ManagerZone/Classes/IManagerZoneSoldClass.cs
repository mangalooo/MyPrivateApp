
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels.Games.ManagerZone;
using MyPrivateApp.Data.Models.Games.ManagerZone;

namespace MyPrivateApp.Components.Games.ManagerZone.Classes
{
    public interface IManagerZoneSoldClass
    {
        string Add(ApplicationDbContext db, ManagerZoneSoldViewModels vm);
        string Edit(ApplicationDbContext db, ManagerZoneSoldViewModels vm);
        string Delete(ApplicationDbContext db, ManagerZoneSoldViewModels vm);
        ManagerZoneSoldViewModels ChangeFromModelToViewModel(ManagerZoneSoldPlayers model);
    }
}
