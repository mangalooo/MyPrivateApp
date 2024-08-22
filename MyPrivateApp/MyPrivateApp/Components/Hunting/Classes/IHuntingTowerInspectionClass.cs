
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingTowerInspectionClass
    {
        string Add(ApplicationDbContext db, HuntingTowerInspectionViewModels vm);
        string Edit(ApplicationDbContext db, HuntingTowerInspectionViewModels vm);
        string Delete(ApplicationDbContext db, HuntingTowerInspectionViewModels vm);
        HuntingTowerInspectionViewModels ChangeFromModelToViewModel(HuntingTowerInspection model);
    }
}
