﻿
using MyPrivateApp.Data;
using MyPrivateApp.Components.ViewModels;
using MyPrivateApp.Data.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.FarmWork.Classes
{
    public class FarmWorkClass(ApplicationDbContext db, ILogger<FarmWorkClass> logger, IMapper mapper) : IFarmWorkClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<FarmWorkClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<FarmWorks?> Get(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.FarmWorks.FirstOrDefaultAsync(r => r.FarmWorksId == id)
                   ?? throw new Exception("Gårdsarbetet hittades inte i databasen!");
        }
        public async Task<string> Add(FarmWorksViewModels vm)
        {
            if (vm == null || _db == null) 
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue || vm.Place == 0 || vm.Hours == 0)
                return "Inget datum, plats eller timmar ifyllt!";

            try
            {
                FarmWorks model = ChangeFromViewModelToModel(vm);
                await _db.FarmWorks.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att lägg till ett nytt gårdsarbete!");
                return $"Gick inte att lägg till ett nytt gårdsarbete. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Edit(FarmWorksViewModels vm)
        {
            if (vm == null || vm.FarmWorksId <= 0 && _db == null) 
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue && vm.Place != 0 && vm.Hours <= 0)
                return "Inget datum, plats eller timmar ifyllt!";

            try
            {
                FarmWorks? model = await Get(vm.FarmWorksId);

                if (model == null) return "Hittar inte gårdsarbetet i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ändra gårdsarbetet!");
                return $"Gick inte att ändra gårdsarbetet. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(FarmWorksViewModels vm)
        {
            if (vm == null || vm.FarmWorksId <= 0 && _db == null) 
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                FarmWorks model = ChangeFromViewModelToModel(vm);
                _db.ChangeTracker.Clear();
                _db.FarmWorks.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gick inte att ta bort gårdsarbetet!");
                return $"Gick inte att ta bort gårdsarbetet. Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public FarmWorksViewModels ChangeFromModelToViewModel(FarmWorks model)
        {
            ArgumentNullException.ThrowIfNull(model);

            FarmWorksViewModels vm = _mapper.Map<FarmWorksViewModels>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        public FarmWorks ChangeFromViewModelToModel(FarmWorksViewModels vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            FarmWorks model = _mapper.Map<FarmWorks>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }
    }
}