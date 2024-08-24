
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public interface IFarmWorkClass
    {
        string Add(ApplicationDbContext db, FarmWorksViewModels vm, bool import);
        string Edit(ApplicationDbContext db, FarmWorksViewModels vm);
        string Delete(ApplicationDbContext db, FarmWorksViewModels vm, bool import);
        FarmWorksViewModels ChangeFromModelToViewModel(FarmWorks model);
    }
}
