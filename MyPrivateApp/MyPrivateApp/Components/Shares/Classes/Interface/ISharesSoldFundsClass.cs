
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesSoldFundsClass
    {
        Task<string> Add(SharesSoldFundViewModel vm, bool import);
        Task<string> Edit(SharesSoldFundViewModel vm);
        Task<string> Delete(SharesSoldFunds model);
        SharesSoldFundViewModel ChangeFromModelToViewModel(SharesSoldFunds model);
    }
}
