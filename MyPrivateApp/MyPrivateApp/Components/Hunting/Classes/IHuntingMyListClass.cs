
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models.Hunting;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingMyListClass
    {
        string Add(ApplicationDbContext db, HuntingMyListViewModels vm, bool import);
        string Edit(ApplicationDbContext db, HuntingMyListViewModels vm);
        string Delete(ApplicationDbContext db, HuntingMyListViewModels vm, bool import);
        HuntingMyListViewModels ChangeFromModelToViewModel(HuntingMyList model);
    }
}
