
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;
using MyPrivateApp.Components.Shares.Classes.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesInterestRatesClass(ApplicationDbContext db, ILogger<SharesInterestRatesClass> logger, IMapper mapper) : ISharesInterestRatesClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesInterestRatesClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesInterestRates?> Get(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            return await _db.SharesInterestRates.FirstOrDefaultAsync(r => r.InterestRatesId == id)
                ?? throw new Exception("Räntan hittades inte i databasen!");
        }

        public async Task<string> Add(SharesInterestRatesViewModel vm, bool import)
        {
            if (vm == null || _db == null)
                return await HandleError(null, "Lägg till", import, "Hittar ingen data från formuläret eller databasen!");

            if (vm.Date == DateTime.MinValue && vm.TotalAmount <= 0)
                return await HandleError(null, "Lägg till", import, "Du måste fylla i fälten: Datum och Belopp!");

            try
            {
                SharesInterestRates model = ChangeFromViewModelToModel(vm);
                await _db.SharesInterestRates.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesInterestRatesViewModel vm)
        {
            if (vm == null || _db == null || vm.InterestRatesId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            if (vm.Date == DateTime.MinValue && vm.TotalAmount <= 0)
                return "Du måste fylla i fälten: Datum och Belopp!";

            try
            {
                SharesInterestRates? model = await Get(vm.InterestRatesId);

                if (model == null)
                    return "Hittar inte avgiften i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra räntan! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesInterestRates model)
        {
            if (model == null || _db == null || model.InterestRatesId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            try
            {
                _db.ChangeTracker.Clear();
                _db.SharesInterestRates.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort räntan! Felmeddelande: {ex.Message}";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }


        public SharesInterestRatesViewModel ChangeFromModelToViewModel(SharesInterestRates model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesInterestRatesViewModel vm = _mapper.Map<SharesInterestRatesViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            return vm;
        }

        public SharesInterestRatesViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesInterestRatesViewModel vm = new()
            {
                Account = model.AccountNumber,
                Currency = model.Currency,
                Date = date,
                TotalAmount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero),
                TypeOfTransaction = model.TypeOfTransaction
            };

            return vm;
        }

        private SharesInterestRates ChangeFromViewModelToModel(SharesInterestRatesViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesInterestRates model = _mapper.Map<SharesInterestRates>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            model.TotalAmount = double.Round(vm.TotalAmount, 2, MidpointRounding.AwayFromZero);

            return model;
        }

        private async Task<string> HandleError(SharesInterestRatesViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesInterestRatesViewModel? vm, string type, bool import, string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(vm);

            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    TypeOfTransaction = vm.TypeOfTransaction,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} RÄNTA: " +
                       $"\r\nDatum: {vm.Date} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nId: {vm.InterestRatesId}"
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