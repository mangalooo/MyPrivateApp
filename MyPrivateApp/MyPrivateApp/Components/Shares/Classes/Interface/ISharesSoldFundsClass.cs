
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesSoldFundsClass
    {
        string Add(SharesSoldFundViewModel vm, bool import);
        string Edit(SharesSoldFundViewModel vm);
        string Delete(SharesSoldFunds model);
        SharesSoldFundViewModel ChangeFromModelToViewModel(SharesSoldFunds model);
    }
}
