using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesImportsFileClass
    {
        void Add(ApplicationDbContext db, SharesImportsFileViewModel vm, bool import);
        void Edit(ApplicationDbContext db, SharesImportsFileViewModel vm);
        void Delete(ApplicationDbContext db, SharesImportsFileViewModel vm, bool import);
        SharesImportsFileViewModel ChangeFromModelToViewModel(SharesImportsFile model);
    }
}