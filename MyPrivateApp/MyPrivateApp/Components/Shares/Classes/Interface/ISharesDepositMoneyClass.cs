
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesDepositMoneyClass
    {
        Task<string> Add(SharesDepositMoneyViewModel vm, bool import);
        Task<string> Edit(SharesDepositMoneyViewModel vm);
        Task<string> Delete(SharesDepositMoneyViewModel vm);
        SharesDepositMoneyViewModel ChangeFromModelToViewModel(SharesDepositMoney model);
        SharesDepositMoneyViewModel ChangeFromImportToViewModel(SharesImports model);
        Task<SharesTotalAmounts?> GetTotalAmount(int? id);
    }
}