using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesDividendClass
    {
        void Add(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        void Edit(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        void Delete(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model);
        SharesDividendViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}