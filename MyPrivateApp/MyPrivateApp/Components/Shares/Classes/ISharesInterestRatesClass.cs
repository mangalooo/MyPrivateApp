using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesInterestRatesClass
    {
        string Add(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import);
        string Edit(ApplicationDbContext db, SharesInterestRatesViewModel vm);
        string Delete(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import);
        SharesInterestRatesViewModel ChangeFromModelToViewModel(SharesInterestRates model);
        SharesInterestRatesViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}