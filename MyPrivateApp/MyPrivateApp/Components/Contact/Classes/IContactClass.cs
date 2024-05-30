using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Contact.Classes
{
    public interface IContactClass
    {
        string Add(ApplicationDbContext db, ContactsViewModels vm, bool import);
        string Edit(ApplicationDbContext db, ContactsViewModels vm);
        string Delete(ApplicationDbContext db, ContactsViewModels vm, bool import);
        ContactsViewModels ChangeFromModelToViewModel(Contacts model);
    }
}
