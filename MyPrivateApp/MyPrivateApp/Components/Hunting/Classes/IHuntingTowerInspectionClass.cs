
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingTowerInspectionClass
    {
        Task<string> Add(HuntingTowerInspectionViewModels vm);
        Task<string> Edit(HuntingTowerInspectionViewModels vm);
        Task<string> Delete(HuntingTowerInspection model);
        HuntingTowerInspectionViewModels ChangeFromModelToViewModel(HuntingTowerInspection model);
    }
}
