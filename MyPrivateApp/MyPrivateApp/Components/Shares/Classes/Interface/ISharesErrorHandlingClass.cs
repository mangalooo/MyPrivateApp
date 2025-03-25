
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesErrorHandlingClass
    {
        Task<string> Edit(SharesErrorHandlingViewModel vm);
        Task<string> Delete(SharesErrorHandlings model);
        SharesErrorHandlingViewModel ChangeFromModelToViewModel(SharesErrorHandlings model);
    }
}