using AutoMapper;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class ContactMappingProfileClass : Profile
    {
        public ContactMappingProfileClass()
        {
            CreateMap<ContactsViewModels, Contacts>();
            CreateMap<Contacts, ContactsViewModels>();
        }
    }
}