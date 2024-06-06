using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Contact.Classes
{
    public interface IContactClass
    {
        string Add(ApplicationDbContext db, ContactsViewModels vm);
        string Edit(ApplicationDbContext db, ContactsViewModels vm);
        string Delete(ApplicationDbContext db, ContactsViewModels vm);
        ContactsViewModels ChangeFromModelToViewModel(Contacts model);
    }
}
