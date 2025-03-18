using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesSoldFundsClass
    {
        string Add(ApplicationDbContext db, SharesSoldFundViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesSoldFundViewModel vm, bool import);
        string Delete(ApplicationDbContext db, SharesSoldFundViewModel vm, bool import);
        SharesSoldFundViewModel ChangeFromModelToViewModel(SharesSoldFunds model);
    }
}
