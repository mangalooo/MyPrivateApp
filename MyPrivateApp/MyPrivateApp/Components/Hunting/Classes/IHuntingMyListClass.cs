
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingMyListClass
    {
        Task<string> Add(HuntingMyListViewModels vm);
        Task<string> Edit(HuntingMyListViewModels vm);
        Task<string> Delete(HuntingMyListViewModels vm);
        HuntingMyListViewModels ChangeFromModelToViewModel(HuntingMyList model);
    }
}
