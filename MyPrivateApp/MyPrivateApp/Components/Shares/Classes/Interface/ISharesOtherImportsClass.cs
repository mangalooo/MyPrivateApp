
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesOtherImportsClass
    {
        Task<string> Add(SharesOtherShareImportViewModel vm, bool import);
        Task<string> Edit(SharesOtherShareImportViewModel vm);
        Task<string> Delete(SharesOtherImports model);
        SharesOtherShareImportViewModel ChangeFromModelToViewModel(SharesOtherImports model);
        SharesOtherShareImportViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}