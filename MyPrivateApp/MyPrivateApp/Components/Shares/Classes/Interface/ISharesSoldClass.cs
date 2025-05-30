
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesSoldClass
    {
        Task<string> Add(SharesSoldViewModel vm, bool import);
        Task<string> Edit(SharesSoldViewModel vm);
        Task<string> Delete(SharesSolds model);
        SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model);
    }
}
