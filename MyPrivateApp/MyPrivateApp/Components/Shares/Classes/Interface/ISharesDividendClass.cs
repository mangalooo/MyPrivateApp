
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesDividendClass
    {
        Task<string> Add(SharesDividendViewModel vm, bool import);
        Task<string> Edit(SharesDividendViewModel vm);
        Task<string> Delete(SharesDividendViewModel vm);
        SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model);
        SharesDividendViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}