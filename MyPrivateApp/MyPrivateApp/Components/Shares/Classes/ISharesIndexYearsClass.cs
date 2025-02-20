using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesIndexYearsClass
    {
        Task<SharesTotalProfitsOrLosses?> GetTotalProfitsOrLossesAsync(ApplicationDbContext db, int? id);
        SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model);
        string CalculateLastYearsResults(ApplicationDbContext db);
        string Delete(ApplicationDbContext db, SharesProfitOrLossYearViewModel vm);
    }
}
