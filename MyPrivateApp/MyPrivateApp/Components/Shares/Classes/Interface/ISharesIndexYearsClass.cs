
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesIndexYearsClass
    {
        Task<SharesTotalProfitsOrLosses> GetTotalProfitsOrLosses(int? id);
        SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model);
        Task<string> CalculateLastYearsResults();
        Task<string> Delete(SharesProfitOrLossYears model);
    }
}
