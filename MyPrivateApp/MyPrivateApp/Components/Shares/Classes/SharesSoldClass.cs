
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesSoldClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesSoldClass> logger, IMapper mapper) : ISharesSoldClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesSoldClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesSolds?> Get(string ISIN)
        {
            if (string.IsNullOrEmpty(ISIN))
                throw new Exception("Get: Finns inget ISIN!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Get: db == null!");

            return await db.SharesSolds.FirstOrDefaultAsync(r => r.ISIN == ISIN)
                ?? throw new Exception("Den sålda aktien hittades inte i databasen!");
        }



        public async Task<string> Add(SharesSoldViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null) return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret!");

                if (IsImportantFieldsSet(vm))
                    return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, pris per såld aktie, Säljdatum och Courage!");

                SharesSolds model = ChangeFromViewModelToModel(vm);

                db.SharesSolds.Add(model);
                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt", import, ex.Message);
            }
        }

        private static bool IsImportantFieldsSet(SharesSoldViewModel vm)
        {
            return vm == null
                ? throw new Exception("IsImportantFieldsSet: vm == null!")
                : vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage > 0 && vm.DateOfSold != DateTime.MinValue && !string.IsNullOrEmpty(vm.PricePerSharesSold);
        }

        public async Task<string> Edit(SharesSoldViewModel vm)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null || vm.SharesSoldId <= 0 || string.IsNullOrEmpty(vm.ISIN))
                    return "Hittar ingen data från formuläret eller ISIN!";

                if (IsImportantFieldsSet(vm))
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, pris per såld aktie, Säljdatum och Courage!";

                SharesSolds? model = await Get(vm.ISIN);

                if (model == null)
                    return "Hittar inte den sålda aktien i databasen!";

                _mapper.Map<SharesSolds>(model);

                model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
                model.DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd");
                model.Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero);
                model.AmountSold = double.Round(vm.HowMany * double.Parse(vm.PricePerSharesSold), 2, MidpointRounding.AwayFromZero);
                model.PricePerShares = double.Round(double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero);
                model.PricePerSharesSold = double.Round(double.Parse(vm.PricePerSharesSold), 2, MidpointRounding.AwayFromZero);
                model.Brokerage = double.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero);
                model.MoneyProfitOrLoss = double.Round(model.AmountSold - model.Amount, 2, MidpointRounding.AwayFromZero);
                double calculateMoneyProfitOrLoss = (model.AmountSold / model.Amount) - 1;
                model.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

                db.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Ändra felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> Delete(SharesSolds model)
        {
            if (model == null || model.SharesSoldId <= 0)
                return "Aktien saknar data i formuläret!";

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear();
                db.SharesSolds.Remove(model);
                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort den sålda aktien! Felmeddelande: {ex.Message} ";
            }
        }

        public SharesSoldViewModel ChangeFromModelToViewModel(SharesSolds model)
        {
            DateTime dateOfPurchase = DateTime.Parse(model.DateOfPurchase);
            DateTime dateOfSold = DateTime.Parse(model.DateOfSold);

            SharesSoldViewModel vm = new()
            {
                SharesSoldId = model.SharesSoldId,
                DateOfPurchase = dateOfPurchase,
                DateOfSold = dateOfSold,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                CompanyName = model.CompanyName,
                HowMany = model.HowMany,
                PricePerShares = double.Round(model.PricePerShares, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PricePerSharesSold = double.Round(model.PricePerSharesSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Brokerage = model.Brokerage,
                TypeOfShares = model.TypeOfShares,
                MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PercentProfitOrLoss = model.PercentProfitOrLoss,
                Note = model.Note
            };

            return vm;
        }

        private static SharesSolds ChangeFromViewModelToModel(SharesSoldViewModel vm)
        {
            SharesSolds sharesSolds = new()
            {
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd"),
                Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(vm.HowMany * double.Parse(vm.PricePerSharesSold), 2, MidpointRounding.AwayFromZero),
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = double.Round(double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero),
                PricePerSharesSold = double.Round(double.Parse(vm.PricePerSharesSold), 2, MidpointRounding.AwayFromZero),
                Brokerage = double.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero),
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Account = vm.Account,
                TypeOfShares = vm.TypeOfShares,
                Note = vm.Note
            };

            sharesSolds.MoneyProfitOrLoss = double.Round(sharesSolds.AmountSold - sharesSolds.Amount, 2, MidpointRounding.AwayFromZero);

            double calculateMoneyProfitOrLoss = (sharesSolds.AmountSold / sharesSolds.Amount) - 1;

            sharesSolds.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

            return sharesSolds;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private async Task<string> HandleError(SharesSoldViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import && vm != null)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesSoldViewModel vm, string type, bool import, string errorMessage)
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
                    CompanyOrInformation = vm.CompanyName,
                    TypeOfTransaction = type + " fond",
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} SÅLD AKTIE: " +
                       $"\r\nKöp datum: {vm.DateOfPurchase.ToString()[..10]} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nId: {vm.SharesSoldId} " +
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