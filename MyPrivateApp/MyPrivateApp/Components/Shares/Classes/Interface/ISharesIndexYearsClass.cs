using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesIndexYearsClass
    {
        SharesTotalProfitsOrLosses? GetTotalProfitsOrLosses(int? id);
        SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model);
        string CalculateLastYearsResults(ApplicationDbContext db);
        string Delete(ApplicationDbContext db, SharesProfitOrLossYearViewModel vm);
    }
}
