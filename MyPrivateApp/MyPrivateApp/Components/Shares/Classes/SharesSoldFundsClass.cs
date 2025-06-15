
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

        private async Task<SharesSoldFunds?> Get(int? id)
        {
            if (id <= 0)
                throw new Exception("Get: Finns inget ID!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Get: db == null!");

            return await db.SharesSoldFunds.FirstOrDefaultAsync(r => r.SharesSoldFundId == id)
                ?? throw new Exception("Den sålda fonden hittades inte i databasen!");
        }

        private static bool IsImportantFieldsSet(SharesSoldFundViewModel vm)
        {
            return vm == null
                ? throw new Exception("IsImportantFieldsSet: vm == null!")
                : vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) && !string.IsNullOrEmpty(vm.ISIN) &&
                    vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage > 0 && vm.DateOfSold != DateTime.MinValue && !string.IsNullOrEmpty(vm.PricePerSharesSold);
        }

        public async Task<string> Add(SharesSoldFundViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null) return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret!");

                if (IsImportantFieldsSet(vm))
                    return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, pris per såld aktie, Säljdatum och Courage!");

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
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                if (vm == null || vm.SharesSoldId <= 0 || string.IsNullOrEmpty(vm.ISIN))
                    return "Hittar ingen data från formuläret eller ISIN!";

                if (IsImportantFieldsSet(vm))
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie, pris per såld aktie, Säljdatum och Courage!";

                SharesSoldFunds? getModel = await Get(vm.ISIN);

                if (getModel == null)
                    return "Hittar inte den sålda aktien i databasen!";

                SharesSoldFunds? model = _mapper.Map<SharesSoldFunds>(getModel);

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
                return $"Ändra. Felmeddelande: {ex.Message}";
            }
        }

        public string Edit(SharesSoldFundViewModel vm)
        {
            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.FundName) && !string.IsNullOrEmpty(vm.ISIN) 
                    && vm.HowMany < 0 && string.IsNullOrEmpty(vm.PricePerFunds) && vm.DateOfSold != DateTime.MinValue && string.IsNullOrEmpty(vm.PricePerFundsSold))
                {
                    try
                    {
                        SharesSoldFunds? dbModel = Get(db, vm.SharesSoldFundId);

                        if (dbModel != null)
                        {
                            SharesSoldFunds model = ChangeFromViewModelToModel(vm);

                            dbModel.DateOfPurchase = model.DateOfPurchase;
                            dbModel.Amount = double.Round(model.HowMany * model.PricePerFunds, 2, MidpointRounding.AwayFromZero);
                            dbModel.DateOfSold = model.DateOfSold;
                            dbModel.AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero);
                            dbModel.FundName = model.FundName;
                            dbModel.HowMany = model.HowMany;
                            dbModel.PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero);
                            dbModel.PricePerFundsSold = double.Round(model.PricePerFundsSold, 2, MidpointRounding.AwayFromZero);
                            dbModel.Currency = model.Currency;
                            dbModel.ISIN = model.ISIN;
                            dbModel.Account = model.Account;
                            dbModel.Fee = model.Fee;
                            dbModel.TypeOfFund = model.TypeOfFund;
                            dbModel.MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero);
                            dbModel.PercentProfitOrLoss = model.PercentProfitOrLoss;
                            dbModel.Note = model.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Fonden hittades inte i databasen!!";
                    }
                    catch (Exception ex)
                    {
                        return $"Ändra felmeddelande: {ex.Message}";
                    }
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per fond del, pris per såld fond och Säljdatum!";
            }
            else
                return "Fonden hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public string Delete(SharesSoldFunds model)
        {
            if (vm != null && !string.IsNullOrEmpty(vm.AmountSold) && db != null)
            {
                SharesSoldFunds model = ChangeFromViewModelToModel(vm);

                try
                {
                    db.ChangeTracker.Clear();
                    db.SharesSoldFunds.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
                }
            }
            else
                return "Fonden hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public SharesSoldFundViewModel ChangeFromModelToViewModel(SharesSoldFunds model)
        {
            DateTime dateOfPurchase = DateTime.Parse(model.DateOfPurchase);
            DateTime dateOfSold = DateTime.Parse(model.DateOfSold);

            SharesSoldFundViewModel vm = new()
            {
                SharesSoldFundId = model.SharesSoldFundId,
                DateOfPurchase = dateOfPurchase,
                DateOfSold = dateOfSold,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                AmountSold = double.Round(model.AmountSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                FundName = model.FundName,
                HowMany = model.HowMany,
                PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PricePerFundsSold = double.Round(model.PricePerFundsSold, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Fee = model.Fee,
                TypeOfFund = model.TypeOfFund,
                MoneyProfitOrLoss = double.Round(model.MoneyProfitOrLoss, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                PercentProfitOrLoss = model.PercentProfitOrLoss,
                Note = model.Note
            };

            return vm;
        }

        private static SharesSoldFunds ChangeFromViewModelToModel(SharesSoldFundViewModel vm)
        {
            SharesSoldFunds sharesSoldFunds = new()
            {
                SharesSoldFundId = vm.SharesSoldFundId,
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                DateOfSold = vm.DateOfSold.ToString("yyyy-MM-dd"),
                Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerFunds), 2, MidpointRounding.AwayFromZero),
                AmountSold = double.Round(vm.HowMany * double.Parse(vm.PricePerFundsSold), 2, MidpointRounding.AwayFromZero),
                FundName = vm.FundName,
                HowMany = vm.HowMany,
                PricePerFunds = double.Round(double.Parse(vm.PricePerFunds), 2, MidpointRounding.AwayFromZero),
                PricePerFundsSold = double.Round(double.Parse(vm.PricePerFundsSold), 2, MidpointRounding.AwayFromZero),
                Fee = vm.Fee,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Account = vm.Account,
                TypeOfFund = vm.TypeOfFund,
                Note = vm.Note
            };

            sharesSoldFunds.MoneyProfitOrLoss = sharesSoldFunds.AmountSold - sharesSoldFunds.Amount;

            double calculateMoneyProfitOrLoss = (sharesSoldFunds.AmountSold / sharesSoldFunds.Amount) - 1;

            sharesSoldFunds.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

            return sharesSoldFunds;
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
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("ErrorHandling: db == null!");

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