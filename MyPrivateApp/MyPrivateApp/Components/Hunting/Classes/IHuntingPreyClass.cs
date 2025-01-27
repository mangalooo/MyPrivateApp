
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingPreyClass
    {
        string Add(ApplicationDbContext db, HuntingPreyViewModels vm, bool import);
        string Edit(ApplicationDbContext db, HuntingPreyViewModels vm);
        string Delete(ApplicationDbContext db, HuntingPreyViewModels vm, bool import);
        HuntingPreyViewModels ChangeFromModelToViewModel(HuntingPrey model);
    }
}
