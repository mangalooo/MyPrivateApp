
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingTeamMemberClass
    {
        Task<string> Add(HuntingTeamMembersViewModels vm);
        Task<string> Edit(HuntingTeamMembersViewModels vm);
        Task<string> Delete(HuntingTeamMembersViewModels vm);
        HuntingTeamMembersViewModels ChangeFromModelToViewModel(HuntingTeamMembers model);
    }
}