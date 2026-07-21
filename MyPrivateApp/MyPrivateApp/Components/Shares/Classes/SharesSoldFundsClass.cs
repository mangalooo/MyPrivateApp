
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldFundsClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesSoldFundsClass> logger) : ISharesSoldFundsClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesSoldFundsClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesSoldFundViewModel vm, bool import)
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Add: db == null!");

                if (vm == null) return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret!");

                if (IsImportantFieldsSet(vm))
                    return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Fondnamn, ISIN, Inköpsdatum, Antal, Pris per fund andel, Pris per såld fond andel, Säljdatum och Avgift!");

                SharesSoldFunds model = ChangeFromViewModelToModel(vm);

                db.SharesSoldFunds.Add(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesSoldFundViewModel vm)
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Edit: db == null!");

                if (vm == null || vm.SharesSoldFundId <= 0 || string.IsNullOrEmpty(vm.ISIN))
                    return "Hittar ingen data från formuläret eller ISIN!";

                if (IsImportantFieldsSet(vm))
                    return "Du måste fylla i fälten: Fondnamn, ISIN, Inköpsdatum, Antal, Pris per fund andel, Pris per såld fond andel, Säljdatum och Avgift!";

                SharesSoldFunds? model = await db.SharesSoldFunds.FirstOrDefaultAsync(r => r.ISIN == vm.ISIN)
                    ?? throw new Exception("Den sålda fonden hittades inte i databasen!");

                if (model == null)
                    return "Hittar inte den sålda aktien i databasen!";

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

        public async Task<string> Delete(SharesSoldFunds model)
        {
            if (model == null || model.SharesSoldFundId <= 0)
                return "Aktien saknar data i formuläret!";

            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear();
                db.SharesSoldFunds.Remove(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort den sålda fonden! Felmeddelande: {ex.Message} ";
            }
        }

        private static bool IsImportantFieldsSet(SharesSoldFundViewModel vm)
        {
            return vm == null
                ? throw new Exception("IsImportantFieldsSet: vm == null!")
                : vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.FundName) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerFunds) && vm.Fee > 0 && vm.DateOfSold != DateTime.MinValue && !string.IsNullOrEmpty(vm.PricePerFundsSold);
        }

        private static DateTime ParseDate(string? date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return DateTime.MinValue;

            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;
        }

        public SharesSoldFundViewModel ChangeFromModelToViewModel(SharesSoldFunds model)
        {
            return new SharesSoldFundViewModel 
            {
                SharesSoldFundId = model.SharesSoldFundId,
                FundName = model.FundName,
                ISIN = model.ISIN,
                TypeOfFund = model.TypeOfFund,
                HowMany = model.HowMany,
                Fee = model.Fee,
                Account = model.Account,
                Currency = model.Currency,
                Note = model.Note,
                DateOfPurchase = ParseDate(model.DateOfPurchase),
                DateOfSold = ParseDate(model.DateOfSold),
                PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PricePerFundsSold = double.Round(model.PricePerFundsSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PercentProfitOrLoss = model.PercentProfitOrLoss
            };
        }

        private static SharesSoldFunds ChangeFromViewModelToModel(SharesSoldFundViewModel vm)
        {
            return new SharesSoldFunds
            {
                SharesSoldFundId = vm.SharesSoldFundId,
                FundName = vm.FundName,
                ISIN = vm.ISIN,
                TypeOfFund = vm.TypeOfFund,
                HowMany = vm.HowMany,
                Fee = vm.Fee,
                Account = vm.Account,
                Currency = vm.Currency,
                Note = vm.Note,
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd"),
                PricePerFunds = double.Round(double.Parse(vm.PricePerFunds ?? "0"), 2, MidpointRounding.AwayFromZero),
                PricePerFundsSold = double.Round(double.Parse(vm.PricePerFundsSold ?? "0"), 2, MidpointRounding.AwayFromZero),
                Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerFunds ?? "0"), 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(vm.HowMany * double.Parse(vm.PricePerFundsSold ?? "0"), 2, MidpointRounding.AwayFromZero),
                MoneyProfitOrLoss = double.Round((vm.HowMany * double.Parse(vm.PricePerFundsSold ?? "0")) - (vm.HowMany * double.Parse(vm.PricePerFunds ?? "0")), 2, MidpointRounding.AwayFromZero),
                PercentProfitOrLoss = ConvertToPercentage(((vm.HowMany * double.Parse(vm.PricePerFundsSold ?? "0")) / (vm.HowMany * double.Parse(vm.PricePerFunds ?? "0"))) - 1)
            };
        }

        private static void EditModel(SharesSoldFunds model, SharesSoldFundViewModel vm)
        {
            model.FundName = vm.FundName;
            model.ISIN = vm.ISIN;
            model.TypeOfFund = vm.TypeOfFund;
            model.HowMany = vm.HowMany;
            model.Fee = vm.Fee;
            model.Account = vm.Account;
            model.Currency = vm.Currency;
            model.Note = vm.Note;
            model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
            model.DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd");
            model.PricePerFunds = double.Round(double.Parse(vm.PricePerFunds ?? "0"), 2, MidpointRounding.AwayFromZero);
            model.PricePerFundsSold = double.Round(double.Parse(vm.PricePerFundsSold ?? "0"), 2, MidpointRounding.AwayFromZero);
            model.Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerFunds ?? "0"), 2, MidpointRounding.AwayFromZero);
            model.AmountSold = double.Round(vm.HowMany * double.Parse(vm.PricePerFundsSold ?? "0"), 2, MidpointRounding.AwayFromZero);
            model.MoneyProfitOrLoss = double.Round((vm.HowMany * double.Parse(vm.PricePerFundsSold ?? "0")) - (vm.HowMany * double.Parse(vm.PricePerFunds ?? "0")), 2, MidpointRounding.AwayFromZero);
            model.PercentProfitOrLoss = ConvertToPercentage(((vm.HowMany * double.Parse(vm.PricePerFundsSold ?? "0")) / (vm.HowMany * double.Parse(vm.PricePerFunds ?? "0"))) - 1);
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private async Task<string> HandleError(SharesSoldFundViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import && vm != null)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesSoldFundViewModel vm, string type, bool import, string errorMessage)
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("ErrorHandling: db == null!");

                if (vm == null)
                    throw new Exception("ErrorHandling: SharesSoldViewModel == null!");

                DateTime date = DateTime.Now;
                string importTrue = import ? "Ja" : "Nej";

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.FundName,
                    TypeOfTransaction = type,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} SÅLD FOND: " +
                           $"\r\nDatum: {vm.DateOfPurchase} " +
                           $"\r\nImport: {importTrue} " +
                           $"\r\nId: {vm.SharesSoldFundId} " +
                           $"\r\nISIN: {vm.ISIN}."
                };

                await db.SharesErrorHandlings.AddAsync(sharesErrorHandling);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Såld aktie: Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }

        public async Task<(double TotalMoneyProfitOrLoss, int Count)> GetTotalMoneyProfitOrLossAndCount()
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("GetTotalMoneyProfitOrLossAndCount: db == null!");

                var result = await db.SharesSoldFunds
                    .GroupBy(x => 1)
                    .Select(g => new
                    {
                        TotalMoneyProfitOrLoss = g.Sum(x => x.MoneyProfitOrLoss),
                        Count = g.Count()
                    })
                    .FirstOrDefaultAsync();

                return result == null
                    ? (0, 0)
                    : (double.Round(result.TotalMoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero), result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get total MoneyProfitOrLoss and count from SharesSoldFunds.");
                return (0, 0);
            }
        }
    }
}