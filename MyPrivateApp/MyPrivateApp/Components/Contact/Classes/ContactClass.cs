
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class ContactClass : IContactClass
    {
        private static Contacts? Get(ApplicationDbContext db, int? id) => db.Contacts.Any(r => r.ContactsId == id) ?
                                                                               db.Contacts.FirstOrDefault(r => r.ContactsId == id) :
                                                                                   throw new Exception("Kontakten hittades inte i databasen!");

        public string Add(ApplicationDbContext db, ContactsViewModels vm, bool import)
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
                        ErrorHandling(db, vm, "Lägg till", import, ex.Message);
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Lägg till", import, "Ingen name eller födelsedag ifyllt!");
                    else
                        return "Ingen datum ifyllt eller plats är i fyllt!";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Lägg till", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            }

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
                        ErrorHandling(db, vm, "Ändra", false, ex.Message);
                    }
                }
                else
                    return "Ingen name eller födelsedag ifyllt!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, ContactsViewModels vm, bool import)
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
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
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

        private static void ErrorHandling(ApplicationDbContext db, ContactsViewModels vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} kontakt: {DateTime.Now}. Namn: {vm.Name}, Födelsedag: {vm.Birthday}. "
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}
