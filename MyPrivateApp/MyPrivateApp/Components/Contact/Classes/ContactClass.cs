
using Hangfire;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Components.Email.Classes;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class ContactClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<ContactClass> logger, IConfiguration config, IEmailSender emailSender) : IContactClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<ContactClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
        private readonly IEmailSender _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));

        public async Task<string> Add(ContactsViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (vm.Birthday == DateTime.MinValue || string.IsNullOrEmpty(vm.Name))
                return "Ingen namn eller födelsedag ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                Contacts model = ChangeFromViewModelToModel(vm);

                await db.Contacts.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ny kontakt.");
                return $"Gick inte att lägg till ny kontakt. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(ContactsViewModels vm)
        {
            if (vm == null || vm.ContactsId <= 0)
                return "Hittar ingen data från formuläret!";

            if (vm.Birthday == DateTime.MinValue || string.IsNullOrEmpty(vm.Name))
                return "Ingen namn eller födelsedag ifyllt!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                Contacts? model = await db.Contacts.FirstOrDefaultAsync(r => r.ContactsId == vm.ContactsId);
                if (model == null)
                    return "Hittar inte kontakten i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra kontakten!");
                return $"Gick inte att ändra kontakten! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(Contacts model)
        {
            if (model == null || model.ContactsId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.Contacts.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort kontakten!");
                return $"Gick inte att ta bort kontakten! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public ContactsViewModels ChangeFromModelToViewModel(Contacts model)
        {
            ContactsViewModels vm = new()
            {
                ContactsId = model.ContactsId,
                Name = model.Name,
                Birthday = ParseDate(model.Birthday ?? string.Empty),
                Address = model.Address,
                PostCode = model.PostCode,
                City = model.City,
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
                HomePage = model.HomePage,
                Notes = model.Notes,
                ChristmasCard = model.ChristmasCard,
                Relatives = model.Relatives,
                Friends = model.Friends,
                Colleagues = model.Colleagues
            };

            return vm;
        }

        public Contacts ChangeFromViewModelToModel(ContactsViewModels vm)
        {
            Contacts model = new()
            {
                ContactsId = vm.ContactsId,
                Name = vm.Name,
                Birthday = vm.Birthday != DateTime.MinValue ? vm.Birthday.ToString("yyyy-MM-dd") : string.Empty,
                Address = vm.Address,
                PostCode = vm.PostCode,
                City = vm.City,
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
                HomePage = vm.HomePage,
                Notes = vm.Notes,
                ChristmasCard = vm.ChristmasCard,
                Relatives = vm.Relatives,
                Friends = vm.Friends,
                Colleagues = vm.Colleagues
            };

            return model;
        }

        private static void EditModel(Contacts model, ContactsViewModels vm)
        {
            model.ContactsId = vm.ContactsId;
            model.Name = vm.Name;
            model.Birthday = vm.Birthday != DateTime.MinValue ? vm.Birthday.ToString("yyyy-MM-dd") : null;
            model.Address = vm.Address;
            model.PostCode = vm.PostCode;
            model.City = vm.City;
            model.MarriedPartner = vm.MarriedPartner;
            model.ChildOne = vm.ChildOne;
            model.ChildTwo = vm.ChildTwo;
            model.ChildThree = vm.ChildThree;
            model.ChildFour = vm.ChildFour;
            model.PrivateMail = vm.PrivateMail;
            model.WorkEMail = vm.WorkEMail;
            model.ExtraMail = vm.ExtraMail;
            model.PhoneNumber = vm.PhoneNumber;
            model.HomePhoneNumber = vm.HomePhoneNumber;
            model.WorkPhoneNumber = vm.WorkPhoneNumber;
            model.ExtraPhoneNumber = vm.ExtraPhoneNumber;
            model.HomePage = vm.HomePage;
            model.Notes = vm.Notes;
            model.ChristmasCard = vm.ChristmasCard;
            model.Relatives = vm.Relatives;
            model.Friends = vm.Friends;
            model.Colleagues = vm.Colleagues;
        }

        public async Task GetBirthday()
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("GetBirthday: db == null!");

                DateTime today = DateTime.Now;
                List<Contacts> contactsWithBirthdayToday = await db.Contacts
                    .Where(c => c.Birthday != null)
                    .ToListAsync();

                contactsWithBirthdayToday = [.. contactsWithBirthdayToday.Where
                (
                    c => DateTime.TryParse(c.Birthday, out DateTime birthday) && birthday.Month == today.Month && birthday.Day == today.Day
                )];

                string? mailBirthday = _config.GetSection("AppSettings")["mailBirthday"];

                if (string.IsNullOrEmpty(mailBirthday)) return;

                foreach (Contacts item in contactsWithBirthdayToday)
                {
                    int year = today.Year - DateTime.Parse(item.Birthday ??
                        throw new InvalidOperationException("Födelsedag kan inte vara null")).Year;

                    BackgroundJob.Schedule(() => _emailSender.SendEmailBirthday
                    (
                        $"{item.Name} {year} år", mailBirthday, "Födelsedag",
                        $"Ring: {item.PhoneNumber}", mailBirthday),
                        new DateTime(today.Year, today.Month, today.Day
                    ));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kontakt: GetBirthday felmeddelande!");
            }
        }
    }
}