﻿
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data;
using Hangfire;
using MyPrivateApp.Components.Email.Classes;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class ContactClass : IContactClass
    {
        private static Contacts? Get(ApplicationDbContext db, int? id) => db.Contacts.Any(r => r.ContactsId == id) ?
                                                                               db.Contacts.FirstOrDefault(r => r.ContactsId == id) :
                                                                                   throw new Exception("Kontakten hittades inte i databasen!");

        public string Add(ApplicationDbContext db, ContactsViewModels vm)
        {
            if (vm != null && db != null)
            {
                if (vm.Birthday != DateTime.MinValue && !string.IsNullOrEmpty(vm.Name))
                {
                    try
                    {
                        Contacts model = ChangeFromViewModelToModel(vm);

                        db.Contacts.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att lägg till ny kontakt. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen namn eller födelsedag ifyllt!";

            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, ContactsViewModels vm)
        {
            if (vm != null && vm.ContactsId > 0 && db != null)
            {
                if (vm.Birthday != DateTime.MinValue && !string.IsNullOrEmpty(vm.Name))
                {
                    try
                    {
                        Contacts getDbModel = Get(db, vm.ContactsId);

                        if (getDbModel != null)
                        {
                            getDbModel.Name = vm.Name;
                            getDbModel.Birthday = vm.Birthday.ToString("yyyy-MM-dd");
                            getDbModel.Address = vm.Address;
                            getDbModel.PostCode = vm.PostCode;
                            getDbModel.MarriedPartner = vm.MarriedPartner;
                            getDbModel.ChildOne = vm.ChildOne;
                            getDbModel.ChildTwo = vm.ChildTwo;
                            getDbModel.ChildThree = vm.ChildThree;
                            getDbModel.ChildFour = vm.ChildFour;
                            getDbModel.PrivateMail = vm.PrivateMail;
                            getDbModel.WorkEMail = vm.WorkEMail;
                            getDbModel.ExtraMail = vm.ExtraMail;
                            getDbModel.PhoneNumber = vm.PhoneNumber;
                            getDbModel.HomePhoneNumber = vm.HomePhoneNumber;
                            getDbModel.WorkPhoneNumber = vm.WorkPhoneNumber;
                            getDbModel.ExtraPhoneNumber = vm.ExtraPhoneNumber;
                            getDbModel.City = vm.City;
                            getDbModel.HomePage = vm.HomePage;
                            getDbModel.Notes = vm.Notes;
                            getDbModel.ChristmasCard = vm.ChristmasCard;
                            getDbModel.Friends = vm.Friends;
                            getDbModel.Relatives = vm.Relatives;
                            getDbModel.Colleagues = vm.Colleagues;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Gick inte att ändra kontakten. Felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Ingen name eller födelsedag ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, ContactsViewModels vm)
        {
            if (vm != null && vm.ContactsId > 0 && db != null)
            {
                try
                {
                    Contacts model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.Contacts.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Gick inte att ta bort kontakten. Felmeddelande: {ex.Message}";
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public ContactsViewModels ChangeFromModelToViewModel(Contacts model)
        {
            DateTime birthday = DateTime.Parse(model.Birthday);

            ContactsViewModels contact = new()
            {
                ContactsId = model.ContactsId,
                Name = model.Name,
                Birthday = birthday,
                Address = model.Address,
                PostCode = model.PostCode,
                MarriedPartner = model.MarriedPartner,
                ChildOne = model.ChildOne,
                ChildTwo = model.ChildTwo,
                ChildThree = model.ChildThree,
                ChildFour = model.ChildFour,
                PrivateMail = model.PrivateMail,
                WorkEMail = model.WorkEMail,
                ExtraMail = model.ExtraMail,
                PhoneNumber = model.PhoneNumber,
                HomePhoneNumber = model.HomePhoneNumber,
                WorkPhoneNumber = model.WorkPhoneNumber,
                ExtraPhoneNumber = model.ExtraPhoneNumber,
                City = model.City,
                HomePage = model.HomePage,
                Notes = model.Notes,
                ChristmasCard = model.ChristmasCard,
                Friends = model.Friends,
                Relatives = model.Relatives,
                Colleagues = model.Colleagues
            };

            return contact;
        }

        private static Contacts ChangeFromViewModelToModel(ContactsViewModels vm)
        {
            Contacts contact = new()
            {
                ContactsId = vm.ContactsId,
                Name = vm.Name,
                Birthday = vm.Birthday.ToString("yyyy-MM-dd"),
                Address = vm.Address,
                PostCode = vm.PostCode,
                MarriedPartner = vm.MarriedPartner,
                ChildOne = vm.ChildOne,
                ChildTwo = vm.ChildTwo,
                ChildThree = vm.ChildThree,
                ChildFour = vm.ChildFour,
                PrivateMail = vm.PrivateMail,
                WorkEMail = vm.WorkEMail,
                ExtraMail = vm.ExtraMail,
                PhoneNumber = vm.PhoneNumber,
                HomePhoneNumber = vm.HomePhoneNumber,
                WorkPhoneNumber = vm.WorkPhoneNumber,
                ExtraPhoneNumber = vm.ExtraPhoneNumber,
                City = vm.City,
                HomePage = vm.HomePage,
                Notes = vm.Notes,
                ChristmasCard = vm.ChristmasCard,
                Friends = vm.Friends,
                Relatives = vm.Relatives,
                Colleagues = vm.Colleagues
            };

            return contact;
        }

        public void GetBirthday(ApplicationDbContext db)
        {
            foreach (Contacts item in db.Contacts)
            {
                DateTime date = Convert.ToDateTime(item.Birthday);

                if (DateTime.Now.Month == date.Month && DateTime.Now.Day == date.Day)
                {
                    int year = DateTime.Now.Year - date.Year;

                    EmailSender emailSender = new();

                    IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
                    string mailBirthday = config.GetSection("AppSettings")["mailBirthday"];

                    if (!string.IsNullOrEmpty(mailBirthday))
                    {
                        BackgroundJob.Schedule(() => emailSender.SendEmailBirthday(
                        item.Name + " " + year.ToString() + " år", mailBirthday, "Födelsedag",
                        "Ring: " + item.PhoneNumber, mailBirthday),
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                    }
                }
            }
        }
    }
}