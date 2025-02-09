using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesFeeClass
    {
        string Add(ApplicationDbContext db, SharesFeeViewModel vm, bool import, string soldDate);
        string Edit(ApplicationDbContext db, SharesFeeViewModel vm);
        string Delete(ApplicationDbContext db, SharesFeeViewModel vm, bool import);
        SharesFeeViewModel ChangeFromModelToViewModel(SharesFee model);
        SharesFeeViewModel ChangeFromImportAddToViewModel(SharesImports import);
    }
}