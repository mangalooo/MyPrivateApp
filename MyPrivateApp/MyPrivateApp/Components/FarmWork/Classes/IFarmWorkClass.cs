
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public interface IFarmWorkClass
    {
        Task<string> Add(FarmWorksViewModels vm);
        Task<string> Edit(FarmWorksViewModels vm);
        Task<string> Delete(FarmWorksViewModels vm);
        FarmWorksViewModels ChangeFromModelToViewModel(FarmWorks model);
    }
}