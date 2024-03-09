using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesInterestRatesClass
    {
        void Add(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import);  
        void Edit(ApplicationDbContext db, SharesInterestRatesViewModel vm);
        void Delete(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import);
        SharesInterestRatesViewModel ChangeFromModelToViewModel(SharesInterestRates model);
        SharesInterestRatesViewModel ChangeFromImportToViewModel(SharesImports model);
    }
}