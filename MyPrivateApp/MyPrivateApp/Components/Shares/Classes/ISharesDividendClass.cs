using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public interface ISharesDividendClass
    {
        public void Add(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        public void Edit(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        public void Delete(ApplicationDbContext db, SharesDividendViewModel vm, bool import);
        public SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model);
    }
}