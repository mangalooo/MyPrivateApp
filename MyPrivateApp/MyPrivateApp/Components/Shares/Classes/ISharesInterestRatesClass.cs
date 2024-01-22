using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesInterestRatesClass
    {
        public void Add(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import);  
        public void Edit(ApplicationDbContext db, SharesInterestRatesViewModel vm);
        public void Delete(ApplicationDbContext db, SharesInterestRatesViewModel vm, bool import);
        public SharesInterestRatesViewModel ChangeFromModelToViewModel(SharesInterestRates model);
    }
}