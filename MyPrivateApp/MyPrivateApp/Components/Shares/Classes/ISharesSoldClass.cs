using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesSoldClass
    {
        void Add(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        void Edit(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        void Delete(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model);
    }
}
