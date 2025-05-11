using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesPurchasedFundsClass
    {
        Task<string> Add(SharesPurchasedFundViewModel vm, bool import);
        Task<string> Edit(SharesPurchasedFundViewModel vm);
        Task<string> AddMore(SharesPurchasedFundViewModel vm, bool import);
        Task<string> Sell(SharesPurchasedFundViewModel vm, bool import, ISharesFeeClass sharesFeeClass);
        Task<string> Delete(ApplicationDbContext db, SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import);
        SharesPurchasedFundViewModel ChangeFromModelToViewModel(SharesPurchasedFunds model);
        SharesPurchasedFundViewModel ChangeFromImportSellToViewModel(SharesImports model);
        SharesPurchasedFundViewModel ChangeFromImportAddToViewModel(SharesImports model);
    }
}