using AutoMapper;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<ContactsViewModels, Contacts>();
            CreateMap<Contacts, ContactsViewModels>();
        }
    }
}