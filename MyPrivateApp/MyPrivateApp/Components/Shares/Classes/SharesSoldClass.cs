
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesSoldClass> logger) : ISharesSoldClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesSoldClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesSoldViewModel vm, bool import)
        {
            try
            {
                if (vm == null) 
                    return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret!");

                if (!IsImportantFieldsSet(vm))
                    return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, pris per såld aktie, Säljdatum och Courage!");

                SharesSolds model = ChangeFromViewModelToModel(vm);

                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Add: db == null!");

                db.SharesSolds.Add(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesSoldViewModel vm)
        {
            try
            {
                if (vm == null || vm.SharesSoldId <= 0 || string.IsNullOrEmpty(vm.ISIN))
                    return "Hittar ingen data från formuläret eller ISIN!";

                // Invert the check: return error if important fields are NOT set
                if (!IsImportantFieldsSet(vm))
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, pris per såld aktie, Säljdatum och Courage!";

                await using var db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Edit: db == null!");

                SharesSolds? model = await db.SharesSolds.FirstOrDefaultAsync(r => r.ISIN == vm.ISIN)
                                        ?? throw new Exception("Den sålda aktien hittades inte i databasen!");

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Ändra. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(SharesSolds model)
        {
            if (model == null || model.SharesSoldId <= 0)
                return "Aktien saknar data i formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Delete: db == null!");

                db.SharesSolds.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort den sålda aktien! Felmeddelande: {ex.Message} ";
            }
        }

        private static bool IsImportantFieldsSet(SharesSoldViewModel vm)
        {
            return vm == null
                ? throw new Exception("IsImportantFieldsSet: vm == null!")
                : vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage > 0 && vm.DateOfSold != DateTime.MinValue && !string.IsNullOrEmpty(vm.PricePerSharesSold);
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        private static void EditModel(SharesSolds model, SharesSoldViewModel vm)
        {
            model.CompanyName = vm.CompanyName ?? string.Empty;
            model.ISIN = vm.ISIN ?? string.Empty;
            model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
            model.DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd");
            model.HowMany = vm.HowMany;

            if (double.TryParse(vm.PricePerShares, out double pricePerShares))
                model.PricePerShares = Math.Round(pricePerShares, 2, MidpointRounding.AwayFromZero);
            if (double.TryParse(vm.PricePerSharesSold, out double pricePerSharesSold))
                model.PricePerSharesSold = Math.Round(pricePerSharesSold, 2, MidpointRounding.AwayFromZero);

            model.Amount = Math.Round(vm.HowMany * model.PricePerShares, 2, MidpointRounding.AwayFromZero);
            model.AmountSold = Math.Round(vm.HowMany * model.PricePerSharesSold, 2, MidpointRounding.AwayFromZero);
            model.Brokerage = Math.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero);
            model.MoneyProfitOrLoss = Math.Round(model.AmountSold - model.Amount, 2, MidpointRounding.AwayFromZero);

            double calculateMoneyProfitOrLoss = (model.Amount != 0) ? (model.AmountSold / model.Amount) - 1 : 0;
            model.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);
        }

        public SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model)
        {
            return new SharesSoldViewModel
            {
                SharesSoldId = model.SharesSoldId,
                CompanyName = model.CompanyName,
                ISIN = model.ISIN,
                DateOfPurchase = ParseDate(model.DateOfPurchase),
                DateOfSold = ParseDate(model.DateOfSold),
                HowMany = model.HowMany,
                PricePerShares = model.PricePerShares.ToString("#,##0.00"),
                PricePerSharesSold = model.PricePerSharesSold.ToString("#,##0.00"),
                Amount = model.Amount.ToString("#,##0.00"),
                AmountSold = model.AmountSold.ToString("#,##0.00"),
                Brokerage = model.Brokerage,
                MoneyProfitOrLoss = model.MoneyProfitOrLoss.ToString("#,##0.00"),
                PercentProfitOrLoss = model.PercentProfitOrLoss,
                Note = model.Note,
                TypeOfShares = model.TypeOfShares,
                Currency = model.Currency,
                Account = model.Account
            };
        }

        private static SharesSolds ChangeFromViewModelToModel(SharesSoldViewModel vm)
        {
            SharesSolds model = new()
            {
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd"),  
                Amount = vm.Amount != null && double.TryParse(vm.Amount, out double amount) ? double.Round(amount, 2, MidpointRounding.AwayFromZero) : 0,
                AmountSold = vm.AmountSold != null && double.TryParse(vm.AmountSold, out double amountSold) ? double.Round(amountSold, 2, MidpointRounding.AwayFromZero) : 0,
                PricePerShares = vm.PricePerShares != null && double.TryParse(vm.PricePerShares, out double pricePerShares) ? double.Round(pricePerShares, 2, MidpointRounding.AwayFromZero) : 0,
                PricePerSharesSold = vm.PricePerSharesSold != null && double.TryParse(vm.PricePerSharesSold, out double pricePerSharesSold) ? double.Round(pricePerSharesSold, 2, MidpointRounding.AwayFromZero) : 0,
                Brokerage = vm.Brokerage > 0 ? double.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero) : 0,
                Note = vm.Note ?? string.Empty,
                TypeOfShares = vm.TypeOfShares ?? string.Empty,
                Currency = vm.Currency ?? string.Empty,
                Account = vm.Account ?? string.Empty,
                CompanyName = vm.CompanyName ?? string.Empty,
                ISIN = vm.ISIN ?? string.Empty,
                HowMany = vm.HowMany
            };

            model.MoneyProfitOrLoss = double.Round(model.AmountSold - model.Amount, 2, MidpointRounding.AwayFromZero);

            double calculateMoneyProfitOrLoss = (model.AmountSold / model.Amount) - 1;
            model.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

            return model;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private async Task<string> HandleError(SharesSoldViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import && vm != null)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesSoldViewModel vm, string type, bool import, string errorMessage)
        {
            try
            {

                if (vm == null)
                    throw new Exception("ErrorHandling: SharesSoldViewModel == null!");

                DateTime date = DateTime.Now;
                string importTrue = import ? "Ja" : "Nej";

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.CompanyName,
                    TypeOfTransaction = type + " fond",
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} SÅLD AKTIE: " +
                       $"\r\nKöp datum: {vm.DateOfPurchase.ToString()[..10]} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nId: {vm.SharesSoldId} " +
                       $"\r\nISIN: {vm.ISIN}."
                };

                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("ErrorHandling: db == null!");

                await db.SharesErrorHandlings.AddAsync(sharesErrorHandling);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Såld aktie: Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }
    }
}