using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesOtherImportsClass
    {
        public void Add(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        public void Update(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        public void Delete(ApplicationDbContext db, SharesOtherShareImportViewModel vm, bool import);
        public SharesOtherShareImportViewModel ChangeFromModelToViewModel(SharesOtherImports model);
    }
}