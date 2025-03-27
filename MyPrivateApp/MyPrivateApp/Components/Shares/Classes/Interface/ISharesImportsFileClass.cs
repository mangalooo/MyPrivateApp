
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesImportsFileClass
    {
        Task<string> Add(SharesImportsFileViewModel vm);
        Task<string> Edit(SharesImportsFileViewModel vm);
        Task<string> Delete(SharesImportsFile model);
        SharesImportsFileViewModel ChangeFromModelToViewModel(SharesImportsFile model);
    }
}