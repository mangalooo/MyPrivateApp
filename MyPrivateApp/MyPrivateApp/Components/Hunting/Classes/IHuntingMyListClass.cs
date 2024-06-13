
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

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
