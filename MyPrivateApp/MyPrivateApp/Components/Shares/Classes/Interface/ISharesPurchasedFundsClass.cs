using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesPurchasedFundsClass
    {
        string Add(ApplicationDbContext db, SharesPurchasedFundViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesPurchasedFundViewModel vm);
        string AddMore(ApplicationDbContext db, SharesPurchasedFundViewModel vm, bool import);
        string Sell(ApplicationDbContext db, SharesPurchasedFundViewModel vm, bool import, ISharesFeeClass sharesFeeClass);
        string Delete(ApplicationDbContext db, SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import);
        SharesPurchasedFundViewModel ChangeFromModelToViewModel(SharesPurchasedFunds model);
        SharesPurchasedFundViewModel ChangeFromImportSellToViewModel(SharesImports model);
        SharesPurchasedFundViewModel ChangeFromImportAddToViewModel(SharesImports model);
    }
}