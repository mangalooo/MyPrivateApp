
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesFeeClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesFeeClass> logger, IMapper mapper) : ISharesFeeClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesFeeClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesFee?> Get(int? id)
        {
            if (id <= 0)
                throw new Exception("Get: Finns inget ID!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Get: db == null!");

            return await db.SharesFees.FirstOrDefaultAsync(r => r.SharesFeeId == id)
                ?? throw new Exception("Avgiften/skatten hittades inte i databasen!");
        }

        public async Task<string> Add(SharesFeeViewModel vm, bool import, string soldDate)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null)
                    return await HandleError(vm, "Lägg till", import, "Hittar ingen data från formuläret eller databasen!");

                if (vm.Date == DateTime.MinValue && (vm.Tax <= 0 || vm.Brokerage <= 0 || vm.Fee <= 0))
                    return await HandleError(vm, "Lägg till", import, "Ingen datum ifyllt eller någon av avfigt, skatt eller courtage måste vara mer än 0!");

                SharesFee model = ChangeFromViewModelToModel(vm, soldDate);

                await db.SharesFees.AddAsync(model);
                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesFeeViewModel vm)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                if (vm == null || vm.SharesFeeId <= 0)
                    return "Hittar ingen data från formuläret!";

                if (vm.Date == DateTime.MinValue && (vm.Tax <= 0 || vm.Brokerage <= 0 || vm.Fee <= 0))
                    return "Ingen datum ifyllt eller någon av avgift, skatt eller courtage måste vara mer än 0!";

                SharesFee? model = await Get(vm.SharesFeeId);

                if (model == null)
                    return "Hittar inte avgiften i databasen!";

                _mapper.Map(vm, model);
                await db.SaveChangesAsync();

                return string.Empty;

            }
            catch (Exception ex)
            {
                return $"Gick inte att ändra avgiften! Felmeddelande: {ex.Message} ";
            }
        }

        public async Task<string> Delete(SharesFee model)
        {
            if (model == null || model.SharesFeeId <= 0)
                return "Hittar ingen data från formuläret!";

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear();
                db.SharesFees.Remove(model);
                await db.SaveChangesAsync();

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

        public SharesFeeViewModel ChangeFromImportToViewModel(SharesImports model) // Kan smälla då SharesFeeViewModel vm inte är exakt samma som SharesImports model
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
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("ErrorHandling: db == null!");

                if (vm == null)
                    throw new Exception("ErrorHandling: SharesSoldViewModel == null!");

                DateTime date = DateTime.Now;
                string importTrue = import ? "Ja" : "Nej";

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

                await db.SharesErrorHandlings.AddAsync(sharesErrorHandling);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }
    }
}