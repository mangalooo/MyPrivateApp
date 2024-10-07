
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Trip.Classes
{
    public interface INewModulesClass
    {
        string Add(ApplicationDbContext db, NewModuleViewModel vm);
        string Edit(ApplicationDbContext db, NewModuleViewModel vm);
        string Delete(ApplicationDbContext db, NewModuleViewModel vm);
        NewModuleViewModel ChangeFromModelToViewModel(NewModules model);
    }
}
