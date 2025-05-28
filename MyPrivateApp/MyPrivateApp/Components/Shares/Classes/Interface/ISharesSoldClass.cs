
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesSoldClass
    {
        Task<string> Add(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        Task<string> Edit(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        Task<string> Delete(ApplicationDbContext db, SharesSoldViewModel vm, bool import);
        SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model);
    }
}
