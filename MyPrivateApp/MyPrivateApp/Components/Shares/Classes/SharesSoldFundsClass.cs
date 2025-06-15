
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldFundsClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesSoldFundsClass> logger, IMapper mapper) : ISharesSoldFundsClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesSoldFundsClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesSoldFunds?> Get(string ISIN)
        {
            if (string.IsNullOrEmpty(ISIN))
                throw new Exception("Get: Finns inget ID!");

            await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Get: db == null!");

            return await db.SharesSoldFunds.FirstOrDefaultAsync(r => r.ISIN == ISIN)
                ?? throw new Exception("Den sålda fonden hittades inte i databasen!");
        }

        private static bool IsImportantFieldsSet(SharesSoldFundViewModel vm)
        {
            return vm == null
                ? throw new Exception("IsImportantFieldsSet: vm == null!")
                : vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.FundName) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerFunds) && vm.Fee > 0 && vm.DateOfSold != DateTime.MinValue && !string.IsNullOrEmpty(vm.PricePerFundsSold);
        }

        public async Task<string> Add(SharesSoldFundViewModel vm, bool import)
        {
            try
            {
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null) return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret!");

                if (IsImportantFieldsSet(vm))
                    return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Fondnamn, ISIN, Inköpsdatum, Antal, Pris per fund andel, Pris per såld fond andel, Säljdatum och Avgift!");

                SharesSoldFunds model = ChangeFromViewModelToModel(vm);

                db.SharesSoldFunds.Add(model);
                await db.SaveChangesAsync();

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
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                if (vm == null || vm.SharesSoldFundId <= 0 || string.IsNullOrEmpty(vm.ISIN))
                    return "Hittar ingen data från formuläret eller ISIN!";

                if (IsImportantFieldsSet(vm))
                    return "Du måste fylla i fälten: Fondnamn, ISIN, Inköpsdatum, Antal, Pris per fund andel, Pris per såld fond andel, Säljdatum och Avgift!";

                SharesSoldFunds? getModel = await Get(vm.ISIN);

                if (getModel == null)
                    return "Hittar inte den sålda aktien i databasen!";

                SharesSoldFunds? model = _mapper.Map<SharesSoldFunds>(getModel);

                model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
                model.DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd");
                model.Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerFunds ?? "0"), 2, MidpointRounding.AwayFromZero);
                model.AmountSold = double.Round(vm.HowMany * double.Parse(vm.PricePerFundsSold ?? "0"), 2, MidpointRounding.AwayFromZero);
                model.PricePerFunds = double.Round(double.Parse(vm.PricePerFunds ?? "0"), 2, MidpointRounding.AwayFromZero);
                model.PricePerFundsSold = double.Round(double.Parse(vm.PricePerFundsSold ?? "0"), 2, MidpointRounding.AwayFromZero);
                model.Fee = double.Round(vm.Fee, 2, MidpointRounding.AwayFromZero);
                model.MoneyProfitOrLoss = double.Round(model.AmountSold - model.Amount, 2, MidpointRounding.AwayFromZero);
                double calculateMoneyProfitOrLoss = (model.AmountSold / model.Amount) - 1;
                model.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

                db.SaveChanges();

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
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear();
                db.SharesSoldFunds.Remove(model);
                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort den sålda fonden! Felmeddelande: {ex.Message} ";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesSoldFundViewModel ChangeFromModelToViewModel(SharesSoldFunds model)
        {
            SharesSoldFundViewModel vm = _mapper.Map<SharesSoldFundViewModel>(model);

            if (model.Amount <= 0)
                vm.Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

            if (model.AmountSold <= 0)
                vm.AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

            if (model.PricePerFunds <= 0)
                vm.PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

            if (model.PricePerFundsSold <= 0)
                vm.PricePerFundsSold = double.Round(model.PricePerFundsSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

            if (model.MoneyProfitOrLoss <= 0)
                vm.MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00");

            if (!string.IsNullOrEmpty(model.DateOfPurchase))
                vm.DateOfPurchase = ParseDate(model.DateOfPurchase);

            if (!string.IsNullOrEmpty(model.DateOfSold))
                vm.DateOfSold = ParseDate(model.DateOfSold);

            return vm;
        }

        private SharesSoldFunds ChangeFromViewModelToModel(SharesSoldFundViewModel vm)
        {
            SharesSoldFunds model = _mapper.Map<SharesSoldFunds>(vm);

            if (vm.DateOfPurchase != DateTime.MinValue)
                model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");

            if (vm.DateOfSold != DateTime.MinValue)
                model.DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd");

            if (vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerFunds) && double.TryParse(vm.PricePerFunds, out double pricePerFunds))
                model.Amount = double.Round(vm.HowMany * pricePerFunds, 2, MidpointRounding.AwayFromZero);

            if (vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerFundsSold) && double.TryParse(vm.PricePerFundsSold, out double pricePerFundsSold))
                model.AmountSold = double.Round(vm.HowMany * pricePerFundsSold, 2, MidpointRounding.AwayFromZero);

            if (!string.IsNullOrEmpty(vm.PricePerFunds) && double.TryParse(vm.PricePerFunds, out pricePerFunds))
                model.PricePerFunds = double.Round(pricePerFunds, 2, MidpointRounding.AwayFromZero);

            if (!string.IsNullOrEmpty(vm.PricePerFundsSold) && double.TryParse(vm.PricePerFundsSold, out pricePerFundsSold))
                model.PricePerFundsSold = double.Round(pricePerFundsSold, 2, MidpointRounding.AwayFromZero);

            if (vm.Fee > 0)
                model.Fee = double.Round(vm.Fee, 2, MidpointRounding.AwayFromZero);

            model.MoneyProfitOrLoss = double.Round(model.AmountSold - model.Amount, 2, MidpointRounding.AwayFromZero);

            double calculateMoneyProfitOrLoss = (model.AmountSold / model.Amount) - 1;

            model.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

            return model;
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
                await using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("ErrorHandling: db == null!");

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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Såld aktie: Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }
    }
}