
using MyPrivateApp.Data.Models.Farming;
using MyPrivateApp.Components.ViewModels;

namespace MyPrivateApp.Components.Farming.Classes
{
    public interface IFarmingClass
    {
        Task<string> Add(FarmingViewModels vm);
        Task<string> EditActive(FarmingViewModels vm);
        Task<string> EditInactive(FarmingViewModels vm);
        Task<string> Inactive(FarmingViewModels vm);
        Task<string> DeleteActive(FarmingsActive model);
        Task<string> DeleteInactive(FarmingsInactive model);
        FarmingViewModels ChangeFromModelToViewModel(FarmingsActive model);
        FarmingViewModels ChangeFromModelToViewModel(FarmingsInactive model);
        T ChangeFromViewModelToModel<T>(FarmingViewModels vm);
    }
}
