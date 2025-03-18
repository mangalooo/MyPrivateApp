using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesDividendClass
    {
        string Add(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        string Delete(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model);
        SharesDividendViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}