using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesPurchasedClass
    {
        void Add(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import);
        void Edit(ApplicationDbContext db, SharesPurchasedViewModel vm);
        void AddMore(ApplicationDbContext db, SharesPurchasedViewModel moreSharesPurchased, bool import);
        void Sell(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import, ISharesFeeClass sharesFeeClass); // Säljer hela eller delar av aktien
        void Delete(ApplicationDbContext db, SharesPurchaseds incomingModel, SharesPurchasedViewModel vm, bool import);
        SharesPurchasedViewModel ChangeFromModelToViewModel(SharesPurchaseds model);
        SharesPurchasedViewModel ChangeFromImportSellToViewModel(SharesImports model);
        SharesPurchasedViewModel ChangeFromImportAddToViewModel(SharesImports model);
    }
}