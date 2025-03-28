
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.Shares.Classes.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesOtherImportsClass(ApplicationDbContext db, ILogger<SharesOtherImportsClass> logger, IMapper mapper) : ISharesOtherImportsClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesOtherImportsClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesOtherImports?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.SharesOtherImports.FirstOrDefaultAsync(r => r.OtherImportsId == id)
                ?? throw new Exception("Andra importer hittades inte i databasen!");
        }

        public async Task<string> Add(SharesOtherShareImportViewModel vm, bool import)
        {
            if (vm == null || _db == null)
                return await HandleError(vm, "Lägg till", import, "Hittar ingen data från formuläret eller databasen!");

            if (vm.Date == DateTime.MinValue)
                return await HandleError(vm, "Lägg till", import, "Ingen datum ifyllt!");

            try
            {
                string importTrue = import ? "Ja" : "Nej";
                SharesOtherImports model = ChangeFromViewModelToModel(vm, importTrue);
                await _db.SharesOtherImports.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesOtherShareImportViewModel vm)
        {
            if (vm == null || _db == null || vm.OtherImportsId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            if (vm.Date == DateTime.MinValue)
                return "Ingen datum ifyllt!";

            try
            {
                SharesOtherImports? model = await Get(vm.OtherImportsId);

                if (model == null)
                    return "Hittar inte andra importer i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra andra importer! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesOtherImports model)
        {
            if (model == null || _db == null || model.OtherImportsId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            try
            {
                _db.ChangeTracker.Clear();
                _db.SharesOtherImports.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort andra importer! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesOtherShareImportViewModel ChangeFromModelToViewModel(SharesOtherImports model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesOtherShareImportViewModel vm = _mapper.Map<SharesOtherShareImportViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        public SharesOtherShareImportViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = ParseDate(model.Date);

            SharesOtherShareImportViewModel vm = new()
            {
                Account = model.AccountNumber,
                Company = model.CompanyOrInformation,
                Currency = model.Currency,
                Date = date,
                ISIN = model.ISIN,
                TypeOfTransaction = model.TypeOfTransaction
            };

            if (!string.IsNullOrEmpty(model.AmountString) && model.AmountString != "-")
                vm.Amount = double.Parse(model.AmountString);

            if (!string.IsNullOrEmpty(model.BrokerageString) && model.BrokerageString != "-")
                vm.Brokerage = double.Parse(model.BrokerageString);

            if (!string.IsNullOrEmpty(model.PricePerShareString) && model.PricePerShareString != "-")
                vm.PricePerShare = double.Parse(model.PricePerShareString);

            if (!string.IsNullOrEmpty(model.NumberOfSharesString) && model.NumberOfSharesString != "-")
            {
                string numberOfSharesString = model.NumberOfSharesString;
                double numberOfShares = numberOfSharesString.Contains("-") ? double.Parse(numberOfSharesString[1..]) : double.Parse(numberOfSharesString);

                vm.NumberOfShares = numberOfShares;

                if (numberOfSharesString.Contains('-'))
                    vm.Note = $"Antalet kom med ett - tecken: {model.NumberOfSharesString}";
            }

            return vm;
        }

        private SharesOtherImports ChangeFromViewModelToModel(SharesOtherShareImportViewModel vm, string import)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesOtherImports model = _mapper.Map<SharesOtherImports>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            model.Note = $"Import: {import}\r\n. " + vm.Note;

            return model;
        }

        private async Task<string> HandleError(SharesOtherShareImportViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesOtherShareImportViewModel? vm, string type, bool import, string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(vm);

            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.Company,
                    TypeOfTransaction = vm.TypeOfTransaction,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} ANDRA IMPORTER: \r\nDatum: {vm.Date} \r\nImport: {importTrue}  \r\nISIN: {vm.ISIN}  \r\nId: {vm.OtherImportsId}. "
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