
using MyPrivateApp.Data;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.FrozenFood.Classes
{
    public interface IFrozenFoodClass
    {
        string Add(ApplicationDbContext db, FrozenFoodViewModel vm, bool import);
        string Edit(ApplicationDbContext db, FrozenFoodViewModel vm);
        string Delete(ApplicationDbContext db, FrozenFoodViewModel vm, bool import);
        FrozenFoodViewModel ChangeFromModelToViewModel(FrozenFoods model);
    }
}
