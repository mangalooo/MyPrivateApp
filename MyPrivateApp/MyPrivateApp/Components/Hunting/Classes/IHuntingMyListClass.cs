
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models.Hunting;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingMyListClass
    {
        string Add(ApplicationDbContext db, HuntingViewModels vm, bool import);
        string Edit(ApplicationDbContext db, HuntingViewModels vm);
        string Delete(ApplicationDbContext db, HuntingViewModels vm, bool import);
        HuntingViewModels ChangeFromModelToViewModel(HuntingMyList model);
    }
}
