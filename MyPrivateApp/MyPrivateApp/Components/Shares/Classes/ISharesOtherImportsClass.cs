using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesOtherImportsClass
    {
        void Add(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        void Edit(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        void Delete(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        SharesOtherShareImportViewModel ChangeFromModelToViewModel(SharesOtherImports model);
        SharesOtherShareImportViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}