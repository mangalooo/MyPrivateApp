using MyPrivateApp.Components.ViewModels.FarmWork;
using MyPrivateApp.Data.Models.FarmWork;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public interface IFarmWorksClass
    {
        Task<string> Add(FarmWorksViewModels vm);
        Task<string> Edit(FarmWorksViewModels vm);
        Task<string> Delete(FarmWorks model);
        FarmWorksViewModels ChangeFromModelToViewModel(FarmWorks model);
    }
}