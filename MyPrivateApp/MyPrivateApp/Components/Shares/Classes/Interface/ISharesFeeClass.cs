
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesFeeClass
    {
        Task<string> Add(SharesFeeViewModel vm, bool import);
        Task<string> Edit(SharesFeeViewModel vm);
        Task<string> Delete(SharesFee model);
        SharesFeeViewModel ChangeFromModelToViewModel(SharesFee model);
        SharesFeeViewModel ChangeFromImportToViewModel(SharesImports import);
    }
}