
using MyPrivateApp.Client.ViewModels;
using MyPrivateApp.Data.Models;
using MyPrivateApp.Data;
using Hangfire;
using MyPrivateApp.Components.Email.Classes;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace MyPrivateApp.Components.Contact.Classes
{
    public class ContactClass(ApplicationDbContext db, ILogger<ContactClass> logger, IMapper mapper, IConfiguration config, IEmailSender emailSender) : IContactClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<ContactClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
        private readonly IEmailSender _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));

        public async Task<Contacts?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.Contacts.FirstOrDefaultAsync(r => r.ContactsId == id)
                   ?? throw new Exception("Kontakten hittades inte i databasen!");
        }

        public async Task<string> Add(ContactsViewModels vm)
        {
            if (vm == null) return "Hittar ingen data från formuläret!";

            if (vm.Birthday == DateTime.MinValue || string.IsNullOrEmpty(vm.Name))
                return "Ingen namn eller födelsedag ifyllt!";

            try
            {
                Contacts model = _mapper.Map<Contacts>(vm);
                await _db.Contacts.AddAsync(model);
                await _db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ny kontakt.");
                return "Gick inte att lägg till ny kontakt.";
            }
        }

        public async Task<string> Edit(ContactsViewModels vm)
        {
            if (vm == null || vm.ContactsId <= 0) return "Hittar ingen data från formuläret!";

            if (vm.Birthday == DateTime.MinValue || string.IsNullOrEmpty(vm.Name))
                return "Ingen namn eller födelsedag ifyllt!";

            try
            {
                Contacts? getDbModel = await Get(vm.ContactsId);
                if (getDbModel == null) return "Hittar inte kontakten i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra kontakten.");
                return "Gick inte att ändra kontakten.";
            }
        }

        public async Task<string> Delete(ContactsViewModels vm)
        {
            if (vm == null || vm.ContactsId <= 0) return "Hittar ingen data från formuläret!";

            try
            {
                Contacts model = _mapper.Map<Contacts>(vm);
                _db.ChangeTracker.Clear();
                _db.Contacts.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort kontakten.");
                return "Gick inte att ta bort kontakten.";
            }
        }

        public ContactsViewModels ChangeFromModelToViewModel(Contacts model) => _mapper.Map<ContactsViewModels>(model);

        public async Task GetBirthday()
        {
            try
            {
                DateTime today = DateTime.Now;
                List<Contacts> contactsWithBirthdayToday = await _db.Contacts
                    .Where(c => c.Birthday != null)
                    .ToListAsync();

                contactsWithBirthdayToday = [.. contactsWithBirthdayToday.Where
                    (
                        c => DateTime.TryParse(c.Birthday, out DateTime birthday) && birthday.Month == today.Month && birthday.Day == today.Day
                    )];

                var mailBirthday = _config.GetSection("AppSettings")["mailBirthday"];
                if (string.IsNullOrEmpty(mailBirthday)) return;

                foreach (var item in contactsWithBirthdayToday)
                {
                    var year = today.Year - DateTime.Parse(item.Birthday ?? throw new InvalidOperationException("Birthday cannot be null")).Year;
                    BackgroundJob.Schedule(() => _emailSender.SendEmailBirthday(
                        $"{item.Name} {year} år", mailBirthday, "Födelsedag",
                        $"Ring: {item.PhoneNumber}", mailBirthday),
                        new DateTime(today.Year, today.Month, today.Day));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kontakt: GetBirthday felmeddelande!");
            }
        }
    }
}