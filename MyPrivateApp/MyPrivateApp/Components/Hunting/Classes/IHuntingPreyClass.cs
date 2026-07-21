
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingPreyClass
    {
        Task<string> Add(HuntingPreyViewModels vm);
        Task<string> Edit(HuntingPreyViewModels vm);
        Task<string> Delete(HuntingPrey model);
        HuntingPreyViewModels ChangeFromModelToViewModel(HuntingPrey model);
    }
}
