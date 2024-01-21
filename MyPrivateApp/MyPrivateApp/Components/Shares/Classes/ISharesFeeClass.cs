using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesFeeClass
    {
        void Add(ApplicationDbContext db, SharesFeeViewModel vm, bool import);
        void Edit(ApplicationDbContext db, SharesFeeViewModel vm);
        void Delete(ApplicationDbContext db, SharesFeeViewModel vm, bool import);
        SharesFeeViewModel ChangeFromModelToViewModel(SharesFee model);
    }
}