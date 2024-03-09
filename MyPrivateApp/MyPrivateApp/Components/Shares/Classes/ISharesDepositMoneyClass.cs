using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesDepositMoneyClass
    {
        void Add(ApplicationDbContext db, SharesDepositMoneyViewModel vm, bool import);
        void Edit(ApplicationDbContext db, SharesDepositMoneyViewModel vm);
        void Delete(ApplicationDbContext db, SharesDepositMoneyViewModel vm);
        SharesDepositMoneyViewModel ChangeFromModelToViewModel(SharesDepositMoney model);
        SharesDepositMoneyViewModel ChangeFromImportToViewModel(SharesImports model);
        SharesTotalAmounts GetTotalAmount(ApplicationDbContext db, int? id);
    }
}