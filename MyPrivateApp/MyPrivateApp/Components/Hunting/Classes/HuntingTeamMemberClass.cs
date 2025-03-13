
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingTeamMemberClass(ApplicationDbContext db, ILogger<HuntingTeamMemberClass> logger, IMapper mapper) : IHuntingTeamMemberClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<HuntingTeamMemberClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<HuntingTeamMembers?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.HuntingTeamMembers.FirstOrDefaultAsync(r => r.HuntingTeamMembersId == id)
                   ?? throw new Exception("Medlemmen hittades inte i databasen!");
        }

        public async Task<string> Add(HuntingTeamMembersViewModels vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen medlem i databasen!";

            if (string.IsNullOrEmpty(vm.Name) && string.IsNullOrEmpty(vm.Mail) && string.IsNullOrEmpty(vm.MobilePhone))
                    return "Ingen name, e-post och mobil nummer ifylld!";

            try
            {
                HuntingTeamMembers model = ChangeFromViewModelToModel(vm);
                await _db.HuntingTeamMembers.AddAsync(model);
                await _db.SaveChangesAsync();
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
            if (vm == null || vm.HuntingTeamMembersId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (string.IsNullOrEmpty(vm.Name) && string.IsNullOrEmpty(vm.Mail) && string.IsNullOrEmpty(vm.MobilePhone))
                return "Ingen name, e-post och mobil nummer ifylld!";

            try
            {
                HuntingTeamMembers? getDbModel = await Get(vm.HuntingTeamMembersId);

                if (getDbModel != null) return "Hittar inte medlemmen i databasen!";

                _mapper.Map(vm, getDbModel);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hittar inte medlemmen i databasen!");
                return $"Hittar inte medlemmen i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingTeamMembersViewModels vm)
        {
            if (vm == null || vm.HuntingTeamMembersId <= 0 && _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                HuntingTeamMembers model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.HuntingTeamMembers.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort medlemmmen!");
                return $"Gick inte att ta bort medlemmmen! Felmeddelande: {ex.Message}";
            }
        }

        public HuntingTeamMembersViewModels ChangeFromModelToViewModel(HuntingTeamMembers model) => _mapper.Map<HuntingTeamMembersViewModels>(model);

        private HuntingTeamMembers ChangeFromViewModelToModel(HuntingTeamMembersViewModels vm) => _mapper.Map<HuntingTeamMembers>(vm);
    }
}