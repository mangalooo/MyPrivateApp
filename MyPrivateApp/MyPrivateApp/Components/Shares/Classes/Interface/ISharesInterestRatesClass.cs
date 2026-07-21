
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesInterestRatesClass
    {
        Task<string> Add(SharesInterestRatesViewModel vm, bool import);
        Task<string> Edit(SharesInterestRatesViewModel vm);
        Task<string> Delete(SharesInterestRates vm);
        SharesInterestRatesViewModel ChangeFromModelToViewModel(SharesInterestRates model);
        SharesInterestRatesViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}