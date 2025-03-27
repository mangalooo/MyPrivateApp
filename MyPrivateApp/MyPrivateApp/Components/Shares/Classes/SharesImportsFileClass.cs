
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.Shares.Classes.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesImportsFileClass(ApplicationDbContext db, ILogger<SharesImportsFileClass> logger, IMapper mapper) : ISharesImportsFileClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesImportsFileClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesImportsFile?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.SharesImportsFiles.FirstOrDefaultAsync(r => r.SharesImportsFileId == id)
                ?? throw new Exception("Hittar inte importen i databasen!");
        }

        public async Task<string> Add(SharesImportsFileViewModel vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            if (vm.Date == DateTime.MinValue)
                return "Ingen datum ifyllt!";

            SharesImportsFile model = ChangeFromViewModelToModel(vm);

            try
            {
                await _db.SharesImportsFiles.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Lägg till IMPORTERAD FIL: \r\nFelmeddelande: {ex.Message}.";
            }
        }

        public async Task<string> Edit(SharesImportsFileViewModel vm)
        {
            if (vm == null || _db == null || vm.SharesImportsFileId <= 0)
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            try
            {
                SharesImportsFile? model = await Get(vm.SharesImportsFileId);

                if (model == null)
                    return "Hittar inte importen i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Ändra IMPORTERAD FIL: \r\nFelmeddelande: {ex.Message}.";
            }
        }

        public async Task<string> Delete(SharesImportsFile model)
        {
            if (model == null || _db == null || model.SharesImportsFileId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            try
            {
                _db.ChangeTracker.Clear();
                _db.SharesImportsFiles.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Ta bort IMPORTERAD FIL: \r\nFelmeddelande: {ex.Message}.";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesImportsFileViewModel ChangeFromModelToViewModel(SharesImportsFile model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesImportsFileViewModel vm = _mapper.Map<SharesImportsFileViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        private SharesImportsFile ChangeFromViewModelToModel(SharesImportsFileViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesImportsFile model = _mapper.Map<SharesImportsFile>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }
    }
}