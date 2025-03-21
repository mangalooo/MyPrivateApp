using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesSoldClass
    {
        string Add(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        string Delete(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model);
    }
}
