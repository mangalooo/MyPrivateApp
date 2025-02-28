﻿
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Contact.Classes
{
    public interface IContactClass
    {
        Task<string> Add(ContactsViewModels vm);
        Task<string> Edit(ContactsViewModels vm);
        Task<string> Delete(ContactsViewModels vm);
        ContactsViewModels ChangeFromModelToViewModel(Contacts model);
        Task GetBirthday();
    }
}
