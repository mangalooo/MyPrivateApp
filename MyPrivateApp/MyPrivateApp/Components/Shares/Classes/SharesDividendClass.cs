
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.Shares.Classes.Interface;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesDividendClass(ApplicationDbContext db, ILogger<SharesDividendClass> logger, IMapper mapper) : ISharesDividendClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesDividendClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesDividend?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.SharesDividends.FirstOrDefaultAsync(r => r.DividendId == id)
                ?? throw new Exception("Utdelningen hittades inte i databasen!");
        }

        public async Task<string> Add(SharesDividendViewModel vm, bool import)
        {
            if (vm == null || _db == null)
                return await HandleError(null, "Lägg till", import, "Hittar ingen data från formuläret eller databasen!");

            if (vm.Date == DateTime.MinValue || string.IsNullOrEmpty(vm.Company) || string.IsNullOrEmpty(vm.ISIN)
                || vm.NumberOfShares <= 0 || string.IsNullOrEmpty(vm.PricePerShare))
                return await HandleError(vm, "Lägg till", import, "Du måste fylla i fälten: Inköpsdatum, Företag, ISIN, Antal och Pris per aktie.");

            try
            {
                SharesDividend model = ChangeFromViewModelToModel(vm);
                await _db.SharesDividends.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesDividendViewModel vm)
        {
            if (vm == null || _db == null)
                return "Hittar ingen data från formuläret eller databasen!";

            if (vm.DividendId <= 0 || vm.Date == DateTime.MinValue || string.IsNullOrEmpty(vm.Company)
                || string.IsNullOrEmpty(vm.ISIN) || vm.NumberOfShares <= 0 || string.IsNullOrEmpty(vm.PricePerShare))
                return "Du måste fylla i fälten: Inköpsdatum, Företag, ISIN, Antal och Pris per aktie.";
            
            try
            {
                SharesDividend? model = await Get(vm.DividendId);
                if (model == null)
                    return "Hittar inte utdelningen i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra utdelningen! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesDividend model)
        {
            if (model == null || _db == null || model.DividendId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            try
            {
                _db.ChangeTracker.Clear();
                _db.SharesDividends.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort utdelningen! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesDividendViewModel vm = _mapper.Map<SharesDividendViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            if (model.PricePerShare <= 0)
                vm.PricePerShare = model.PricePerShare.ToString("#,##0.00");

            if (model.TotalAmount <= 0)
                vm.TotalAmount = model.TotalAmount.ToString("#,##0.00");

            return vm;
        }

        public SharesDividendViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesDividendViewModel vm = _mapper.Map<SharesDividendViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            if (!string.IsNullOrEmpty(model.PricePerShareString))
                vm.PricePerShare = double.Parse(model.PricePerShareString).ToString("#,##0.00");

            if (!string.IsNullOrEmpty(model.AmountString))
                vm.TotalAmount = double.Parse(model.AmountString).ToString("#,##0.00");

            return vm;
        }

        public SharesDividend ChangeFromViewModelToModel(SharesDividendViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesDividend model = _mapper.Map<SharesDividend>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            return model;
        }

        private async Task<string> HandleError(SharesDividendViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesDividendViewModel? vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TypeOfTransaction = type,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = vm == null ? null : $"{type} UTDELNING: \r\nDatum: {vm.Date} \r\nImport: {importTrue}  \r\nISIN: {vm.ISIN} \r\nId: {vm.DividendId}. "
                };

                await _db.SharesErrorHandlings.AddAsync(sharesErrorHandling);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }
    }
}