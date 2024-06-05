using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models.Farming;
using MyPrivateApp.Components.ViewModels.SharesViewModels;

namespace MyPrivateApp.Components.Farming.Classes
{
    public interface IFarmingClass
    {
        string Add(ApplicationDbContext db, FarmingViewModels vm, bool import);
        string EditActive(ApplicationDbContext db, FarmingViewModels vm);
        string EditInactive(ApplicationDbContext db, FarmingViewModels vm);
        string Inactive(ApplicationDbContext db, FarmingViewModels vm);
        string DeleteActive(ApplicationDbContext db, FarmingViewModels vm, bool import);
        string DeleteInactive(ApplicationDbContext db, FarmingViewModels vm, bool import);
        FarmingViewModels ChangeFromModelToViewModel(FarmingsActive model);
        FarmingViewModels ChangeFromModelToViewModel(FarmingsInactive model);
    }
}
