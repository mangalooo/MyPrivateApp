using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes.Interface
{
    public interface ISharesDepositMoneyClass
    {
        Task<string> Add(ApplicationDbContext db, SharesDepositMoneyViewModel vm, bool import);
        Task<string> Edit(ApplicationDbContext db, SharesDepositMoneyViewModel vm);
        Task<string> Delete(ApplicationDbContext db, SharesDepositMoneyViewModel vm);
        SharesDepositMoneyViewModel ChangeFromModelToViewModel(SharesDepositMoney model);
        SharesDepositMoneyViewModel ChangeFromImportToViewModel(SharesImports model);
        Task<SharesTotalAmounts?> GetTotalAmount(ApplicationDbContext db, int? id);
    }
}