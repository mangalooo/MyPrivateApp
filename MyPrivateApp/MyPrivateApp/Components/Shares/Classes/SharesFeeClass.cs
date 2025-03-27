
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesFeeClass(ApplicationDbContext db, ILogger<SharesFeeClass> logger, IMapper mapper) : ISharesFeeClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesFeeClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesFee?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.SharesFees.FirstOrDefaultAsync(r => r.SharesFeeId == id)
                ?? throw new Exception("Avgiften/skatten hittades inte i databasen!");
        }

        public async Task<string> Add(SharesFeeViewModel vm, bool import, string soldDate)
        {
            if (vm == null || _db == null)
                return await HandleError(null, "Lägg till", import, "Hittar ingen data från formuläret eller databasen!");

            if (vm.Date == DateTime.MinValue && (vm.Tax <= 0 || vm.Brokerage <= 0 || vm.Fee <= 0))
                return await HandleError(null, "Lägg till", import, "Ingen datum ifyllt eller någon av avfigt, skatt eller courtage måste vara mer än 0!");

            try
            {
                SharesFee model = ChangeFromViewModelToModel(vm, soldDate);
                await _db.SharesFees.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesFeeViewModel vm)
        {
            if (vm == null || _db == null || vm.SharesFeeId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            if (vm.Date == DateTime.MinValue && (vm.Tax <= 0 || vm.Brokerage <= 0 || vm.Fee <= 0))
                return "Ingen datum ifyllt eller någon av avgift, skatt eller courtage måste vara mer än 0!";

            try
            {
                SharesFee? model = await Get(vm.SharesFeeId);

                if (model == null)
                    return "Hittar inte avgiften i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;

            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra avgiften! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesFee model)
        {
            if (model == null || _db == null || model.SharesFeeId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            try
            {
                _db.ChangeTracker.Clear();
                _db.SharesFees.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort avgiften! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesFeeViewModel ChangeFromModelToViewModel(SharesFee model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesFeeViewModel vm = _mapper.Map<SharesFeeViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            if (!string.IsNullOrEmpty(model.DateOfFee))
                vm.DateOfFee = ParseDate(model.DateOfFee);

            return vm;
        }

        public SharesFeeViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesFeeViewModel vm = _mapper.Map<SharesFeeViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            switch (model.TypeOfTransaction)
            {
                case "Skatt":
                    vm.Tax = double.Parse(model.AmountString);
                    break;
                case "Avgift":
                    vm.Fee = double.Parse(model.AmountString);
                    break;                
                case "Courtage":
                    vm.Brokerage = double.Parse(model.AmountString);
                    break;
            }

            return vm;
        }

        private SharesFee ChangeFromViewModelToModel(SharesFeeViewModel vm, string soldDate)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesFee model = _mapper.Map<SharesFee>(vm);

            if (vm.DateOfFee != DateTime.MinValue)
                model.DateOfFee = vm.DateOfFee.ToString("yyyy-MM-dd");

            model.Date = soldDate;
            model.Fee = Math.Round(vm.Fee, 2, MidpointRounding.AwayFromZero);
            model.Tax = Math.Round(vm.Tax, 2, MidpointRounding.AwayFromZero);
            model.Brokerage = Math.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero);

            return model;
        }

        private async Task<string> HandleError(SharesFeeViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesFeeViewModel? vm, string type, bool import, string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(vm);

            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.CompanyOrInformation,
                    TypeOfTransaction = vm.TypeOfTransaction,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} AVGIFTER: " +
                       $"\r\nAvgiftsdatum: {vm.DateOfFee} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nAvgift: {vm.Fee} " +
                       $"\r\nSkatt: {vm.Tax} " +
                       $"\r\nCourtage: {vm.Brokerage} " +
                       $"\r\nKonto: {vm.Account} " +
                       $"\r\nISIN: {vm.ISIN}"
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