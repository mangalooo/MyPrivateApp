using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesIndexYearsClass
    {
        SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model);
        string CalculateLastYearsResults(ApplicationDbContext db);
        string Delete(ApplicationDbContext db, SharesProfitOrLossYearViewModel vm);
    }
}
