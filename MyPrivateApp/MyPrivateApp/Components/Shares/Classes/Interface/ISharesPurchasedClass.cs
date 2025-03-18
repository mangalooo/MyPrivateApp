using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesPurchasedClass
    {
        string Add(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesPurchasedViewModel vm);
        string AddMore(ApplicationDbContext db, SharesPurchasedViewModel moreSharesPurchased, bool import);
        string Sell(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import, ISharesFeeClass sharesFeeClass); // Säljer hela eller delar av aktien
        string Delete(ApplicationDbContext db, SharesPurchaseds incomingModel, SharesPurchasedViewModel vm, bool import);
        SharesPurchasedViewModel ChangeFromModelToViewModel(SharesPurchaseds model);
        SharesPurchasedViewModel ChangeFromImportSellToViewModel(SharesImports model);
        SharesPurchasedViewModel ChangeFromImportAddToViewModel(SharesImports model);
    }
}