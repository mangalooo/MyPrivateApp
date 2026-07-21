
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingTeamMemberClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<HuntingTeamMemberClass> logger) : IHuntingTeamMemberClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<HuntingTeamMemberClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(HuntingTeamMembersViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (string.IsNullOrEmpty(vm.Name) && string.IsNullOrEmpty(vm.Mail) && string.IsNullOrEmpty(vm.MobilePhone))
                    return "Ingen name, e-post och mobil nummer ifylld!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                HuntingTeamMembers model = ChangeFromViewModelToModel(vm);

                await db.HuntingTeamMembers.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till en ny medlem!");
                return $"Gick inte att lägg till en ny medlem! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(HuntingTeamMembersViewModels vm)
        {
            if (vm == null || vm.HuntingTeamMembersId <= 0)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (string.IsNullOrEmpty(vm.Name) && string.IsNullOrEmpty(vm.Mail) && string.IsNullOrEmpty(vm.MobilePhone))
                return "Ingen name, e-post och mobil nummer ifylld!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                HuntingTeamMembers? model = await db.HuntingTeamMembers.FirstOrDefaultAsync(r => r.HuntingTeamMembersId == vm.HuntingTeamMembersId);
                if (model == null) 
                    return "Hittar inte medlemmen i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hittar inte medlemmen i databasen!");
                return $"Hittar inte medlemmen i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingTeamMembers model)
        {
            if (model == null || model.HuntingTeamMembersId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.HuntingTeamMembers.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort medlemmmen!");
                return $"Gick inte att ta bort medlemmmen! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public HuntingTeamMembersViewModels ChangeFromModelToViewModel(HuntingTeamMembers model)
        {
            HuntingTeamMembersViewModels vm = new()
            {
                HuntingTeamMembersId = model.HuntingTeamMembersId,
                Name = model.Name,
                Birthday = model.Birthday != null ? ParseDate(model.Birthday) : DateTime.MinValue,
                Address = model.Address,
                PostCode = model.PostCode,
                City = model.City,
                Mail = model.Mail,
                MobilePhone = model.MobilePhone,
                Note = model.Note
            };

            return vm;
        }

        public HuntingTeamMembers ChangeFromViewModelToModel(HuntingTeamMembersViewModels vm)
        {
            HuntingTeamMembers model = new()
            {
                HuntingTeamMembersId = vm.HuntingTeamMembersId,
                Name = vm.Name,
                Birthday = vm.Birthday != DateTime.MinValue ? vm.Birthday.ToString("yyyy-MM-dd") : null,
                Address = vm.Address,
                PostCode = vm.PostCode,
                City = vm.City,
                Mail = vm.Mail,
                MobilePhone = vm.MobilePhone,
                Note = vm.Note
            };

            return model;
        }

        private static void EditModel(HuntingTeamMembers model, HuntingTeamMembersViewModels vm)
        {
            model.Name = vm.Name;
            model.Birthday = vm.Birthday != DateTime.MinValue ? vm.Birthday.ToString("yyyy-MM-dd") : null;
            model.Address = vm.Address;
            model.PostCode = vm.PostCode;
            model.City = vm.City;
            model.Mail = vm.Mail;
            model.MobilePhone = vm.MobilePhone;
            model.Note = vm.Note;
        }
    }
}