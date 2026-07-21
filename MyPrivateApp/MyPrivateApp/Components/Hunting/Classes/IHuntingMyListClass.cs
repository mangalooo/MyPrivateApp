
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingMyListClass
    {
        Task<string> Add(HuntingMyListViewModels vm);
        Task<string> Edit(HuntingMyListViewModels vm);
        Task<string> Delete(HuntingMyList model);
        HuntingMyListViewModels ChangeFromModelToViewModel(HuntingMyList model);
    }
}
