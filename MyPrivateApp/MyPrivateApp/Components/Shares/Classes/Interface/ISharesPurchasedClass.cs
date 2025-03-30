
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesPurchasedClass
    {
        Task<string> Add(SharesPurchasedViewModel vm, bool import);
        Task<string> Edit(SharesPurchasedViewModel vm);
        Task<string> AddMore(SharesPurchasedViewModel moreSharesPurchased, bool import);
        Task<string> Sell(SharesPurchasedViewModel vm, bool import, ISharesFeeClass sharesFeeClass); // Säljer hela eller delar av aktien
        Task<string> Delete(SharesPurchaseds incomingModel, SharesPurchasedViewModel vm, bool import);
        SharesPurchasedViewModel ChangeFromModelToViewModel(SharesPurchaseds model);
        SharesPurchasedViewModel ChangeFromImportSellToViewModel(SharesImports model);
        SharesPurchasedViewModel ChangeFromImportAddToViewModel(SharesImports model);
    }
}