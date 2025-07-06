
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingTeamMemberClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<HuntingTeamMemberClass> logger, IMapper mapper) : IHuntingTeamMemberClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<HuntingTeamMemberClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

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
                if (model != null) 
                    return "Hittar inte medlemmen i databasen!";

                _mapper.Map(vm, model);
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
            ArgumentNullException.ThrowIfNull(model);

            HuntingTeamMembersViewModels vm = _mapper.Map<HuntingTeamMembersViewModels>(model);

            if (!string.IsNullOrEmpty(model.Birthday))
                vm.Birthday = ParseDate(model.Birthday);

            return vm;
        }

        public HuntingTeamMembers ChangeFromViewModelToModel(HuntingTeamMembersViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            HuntingTeamMembers model = _mapper.Map<HuntingTeamMembers>(vm);

            if (vm.Birthday != DateTime.MinValue)
                model.Birthday = vm.Birthday.ToString("yyyy-MM-dd");

            return model;
        }
    }
}