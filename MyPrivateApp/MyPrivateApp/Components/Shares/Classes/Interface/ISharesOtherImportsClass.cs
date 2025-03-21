using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesOtherImportsClass
    {
        string Add(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        string Delete(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        SharesOtherShareImportViewModel ChangeFromModelToViewModel(SharesOtherImports model);
        SharesOtherShareImportViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}