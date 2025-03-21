using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesImportsFileClass
    {
        string Add(ApplicationDbContext db, SharesImportsFileViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesImportsFileViewModel vm);
        string Delete(ApplicationDbContext db, SharesImportsFileViewModel vm, bool import);
        SharesImportsFileViewModel ChangeFromModelToViewModel(SharesImportsFile model);
    }
}