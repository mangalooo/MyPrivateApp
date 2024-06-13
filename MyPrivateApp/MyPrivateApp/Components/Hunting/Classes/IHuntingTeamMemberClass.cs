
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public interface IHuntingTeamMemberClass
    {
        string Add(ApplicationDbContext db, HuntingTeamMembersViewModels vm, bool import);
        string Edit(ApplicationDbContext db, HuntingTeamMembersViewModels vm);
        string Delete(ApplicationDbContext db, HuntingTeamMembersViewModels vm, bool import);
        HuntingTeamMembersViewModels ChangeFromModelToViewModel(HuntingTeamMembers model);
    }
}
