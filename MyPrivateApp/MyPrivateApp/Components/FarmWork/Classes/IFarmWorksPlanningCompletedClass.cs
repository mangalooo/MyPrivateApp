
using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public interface IFarmWorksPlanningCompletedClass
    {
        Task<string> Edit(FarmWorksPlanningViewModels vm);
        Task<string> Delete(FarmWorksPlanningCompleted FarmWorks);
        FarmWorksPlanningViewModels ChangeFromModelToViewModel(FarmWorksPlanningCompleted model);
    }
}