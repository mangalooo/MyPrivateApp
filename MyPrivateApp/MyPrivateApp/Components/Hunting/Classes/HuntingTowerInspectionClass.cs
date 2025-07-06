
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.Hunting;
using MyPrivateApp.Components.ViewModels.HuntingViemModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Hunting.Classes
{
    public class HuntingTowerInspectionClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<HuntingTowerInspectionClass> logger, IMapper mapper) : IHuntingTowerInspectionClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<HuntingTowerInspectionClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<string> Add(HuntingTowerInspectionViewModels vm)
        {
            if (vm == null)
                return "Hittar ingen data från formuläret!";

            if (string.IsNullOrEmpty(vm.Number))
                return "Du måste fylla i ett pass nummer!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                HuntingTowerInspection model = ChangeFromViewModelToModel(vm);

                await db.HuntingTowerInspections.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ett ny besikning!");
                return $"Gick inte att lägg till ett ny besikning! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(HuntingTowerInspectionViewModels vm)
        {
            if (vm == null || vm.HuntingTowerInspectionId <= 0)
                return "Hittar ingen data från formuläret!";

            if (string.IsNullOrEmpty(vm.Number))
                return "Du måste fylla i ett pass nummer!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                // Fetch the entity in the same context to ensure tracking
                HuntingTowerInspection? model = await db.HuntingTowerInspections.FirstOrDefaultAsync(r => r.HuntingTowerInspectionId == vm.HuntingTowerInspectionId);

                if (model == null) 
                    return "Hittar inte bytet i databasen!";

                _mapper.Map(vm, model);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hittar inte besikningen i databasen!");
                return $"Hittar inte besikningen i databasen! Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(HuntingTowerInspection model)
        {
            if (model == null || model.HuntingTowerInspectionId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.HuntingTowerInspections.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort besikningen!");
                return $"Gick inte att ta bort besikningen! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public HuntingTowerInspectionViewModels ChangeFromModelToViewModel(HuntingTowerInspection model)
        {
            ArgumentNullException.ThrowIfNull(model);

            HuntingTowerInspectionViewModels vm = _mapper.Map<HuntingTowerInspectionViewModels>(model);

            if (!string.IsNullOrEmpty(model.LastInspected))
                vm.LastInspected = ParseDate(model.LastInspected);

            return vm;
        }

        public HuntingTowerInspection ChangeFromViewModelToModel(HuntingTowerInspectionViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            HuntingTowerInspection model = _mapper.Map<HuntingTowerInspection>(vm);

            if (vm.LastInspected != DateTime.MinValue)
                model.LastInspected = vm.LastInspected.ToString("yyyy-MM-dd");

            return model;
        }
    }
}