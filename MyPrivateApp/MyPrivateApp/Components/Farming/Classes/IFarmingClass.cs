
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models.Farming;

namespace MyPrivateApp.Components.Farming.Classes
{
    public interface IFarmingClass
    {
        Task<string> Add(FarmingViewModels vm);
        Task<string> EditActive(FarmingViewModels vm);
        Task<string> EditInactive(FarmingViewModels vm);
        Task<string> Inactive(FarmingViewModels vm);
        Task<string> DeleteActive(FarmingViewModels vm);
        Task<string> DeleteInactive(FarmingViewModels vm);
        FarmingViewModels ChangeFromModelToViewModel(FarmingsActive model);
        FarmingViewModels ChangeFromModelToViewModel(FarmingsInactive model);
        T ChangeFromViewModelToModel<T>(FarmingViewModels vm);
    }
}
