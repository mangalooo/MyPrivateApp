
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public interface IFarmWorksPlanningClass
    {
        Task<string> Add(FarmWorksPlanningViewModels vm);
        Task<string> Edit(FarmWorksPlanningViewModels vm);
        Task<string> Delete(FarmWorksPlanning FarmWorks);
        FarmWorksPlanningViewModels ChangeFromModelToViewModel(FarmWorksPlanning model);
    }
}