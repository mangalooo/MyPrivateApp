
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedFundsClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesPurchasedFundsClass> logger, IMapper mapper) : ISharesPurchasedFundsClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesPurchasedFundsClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesPurchasedFunds?> Get(string ISIN)
        {
            if (string.IsNullOrEmpty(ISIN))
                throw new Exception("Get: Finns inget ISIN!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Get: db == null!");

            return await db.SharesPurchasedFunds.FirstOrDefaultAsync(r => r.ISIN == ISIN)
                ?? throw new Exception("Den köpte fonden hittades inte i databasen!");
        }

        public async Task<string> Add(SharesPurchasedFundViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null)
                    return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret!");

                if (vm.DateOfPurchase == DateTime.MinValue || string.IsNullOrEmpty(vm.FundName) ||
                    string.IsNullOrEmpty(vm.ISIN) || vm.HowMany <= 0 || vm.PricePerFunds <= 0 || vm.Fee <= 0)
                    return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Fond namn, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!");

                SharesPurchasedFunds model = ChangesFromViewModelToModel(vm);

                string importTrue = import ? "Ja" : "Nej";

                model.Note += $"Köper:" +
                              $"\r\nBolag: {model.FundName} aktier" +
                              $"\r\nISIN: {vm.ISIN}";

                if (!string.IsNullOrEmpty(model.DateOfPurchase))
                    model.Note += $"\r\nDatum: {model.DateOfPurchase.ToString()[..10]}";

                model.Note += $"\r\nHur många: {model.HowMany}" +
                              $"\r\nPris per st: {model.PricePerFunds}" +
                              $"\r\nVärdet: {model.Amount}" +
                              $"\r\nAvgift: {model.Fee}" +
                              $"\r\nImport: {importTrue}";

                await db.SharesPurchasedFunds.AddAsync(model);
                await db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesPurchasedFundViewModel vm)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                if (vm == null || db == null || vm.SharesPurchasedFundId <= 0)
                    return "Hittar ingen data från formuläret eller databasen!";

                if (vm.DateOfPurchase == DateTime.MinValue || string.IsNullOrEmpty(vm.FundName) || string.IsNullOrEmpty(vm.ISIN)
                     || vm.HowMany <= 0 || vm.PricePerFunds <= 0 || vm.Fee <= 0)
                    return "Du måste fylla i fälten: Fond namn, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!";

                SharesPurchasedFunds? model = await Get(vm.ISIN);

                if (model == null)
                    return "Hittar inte den köpa fonden i databasen!";

                _mapper.Map(vm, model);
                await db.SaveChangesAsync();
                return string.Empty;

            }
            catch (Exception ex)
            {
                return $"Ändra köpt fond. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> AddMore(SharesPurchasedFundViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("AddMore: db == null!");

                if (vm == null || db == null)
                    return await HandleError(vm, "Köpt mera", import, "Ingen kontakt med VM eller DB för fonden!");

                if (!import && (vm.MoreDateOfPurchase == DateTime.MinValue || vm.MoreHowMany <= 0 || vm.MorePricePerFunds <= 0))
                    return "Du måste fylla i fälten: Köp mer: Datum, Köp mer: Antal och Köp mer: Pris per fond del!";


                if (string.IsNullOrEmpty(vm.ISIN))
                    return await HandleError(vm, "Köpt mera", import, "Finns inget ISIN till fonden!");

                SharesPurchasedFunds? model = await Get(vm.ISIN);

                if (model == null)
                    return await HandleError(vm, "Köpt mera", import, "Hittar inte fonden i databasen!");

                UpdateModelWithAdditionalPurchase(model, vm, import);

                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt mera", import, ex.Message);
            }
        }

        private static void UpdateModelWithAdditionalPurchase(SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import)
        {
            if (import)
            {
                model.HowMany += vm.HowMany;
                model.Fee += vm.Fee;
                model.Amount += vm.HowMany * vm.PricePerFunds;
                model.Note += GenerateNoteForAdditionalPurchase(vm, import, model.Amount);
            }
            else
            {
                model.HowMany += vm.MoreHowMany;
                model.Fee += vm.MoreFee;
                model.Amount += vm.MoreHowMany * vm.MorePricePerFunds;
                model.Note += GenerateNoteForAdditionalPurchase(vm, import, model.Amount);
            }

            model.PricePerFunds = model.Amount / model.HowMany;
        }

        private static string GenerateNoteForAdditionalPurchase(SharesPurchasedFundViewModel vm, bool import, double totalAmount)
        {
            string importTrue = import ? "Ja" : "Nej";
            string date = import ? vm.DateOfPurchase.ToString("yyyy-MM-dd") : vm.MoreDateOfPurchase.ToString("yyyy-MM-dd");
            double howMany = import ? vm.HowMany : vm.MoreHowMany;
            double pricePerFunds = import ? vm.PricePerFunds : vm.MorePricePerFunds;
            double fee = import ? vm.Fee : vm.MoreFee;

            return $"\r\n\r\nKöper mer fond delar för {vm.FundName}: \r\n" +
                   $"Import: {importTrue} \r\n" +
                   $"Datum: {date} \r\n" +
                   $"Hur många: {howMany} \r\n" +
                   $"Pris per st: {pricePerFunds} \r\n" +
                   $"Summan: {totalAmount} \r\n" +
                   $"Courtage: {fee}";
        }

        // Selling all or part of the fund
        public async Task<string> Sell(SharesPurchasedFundViewModel vm, bool import, ISharesFeeClass sharesFeeClass)
        {
            if (vm == null || string.IsNullOrEmpty(vm.ISIN))
                return await HandleError(vm, "Sälj", import, "Formuläret eller ISIN är tomt!");

            SharesPurchasedFunds? fundsPurchased = await Get(vm.ISIN);

            if (fundsPurchased == null)
                return await HandleError(vm, "Sälj", import, "Fonden hittades inte i databasen!");

            vm.SharesPurchasedFundId = fundsPurchased.SharesPurchasedFundId;

            if (!ValidateSaleInput(vm, import))
                return await HandleError(vm, "Sälj", import, "Du måste fylla i alla obligatoriska fält korrekt!");

            string importStatus = import ? "Ja" : "Nej";

            try
            {
                if (fundsPurchased.HowMany == vm.SaleHowMany)
                    // Handle entire share sale
                    return await HandleEntireShareSale(fundsPurchased, vm, importStatus, sharesFeeClass, import);

                else
                    // Handle partial share sale
                    return await HandlePartialShareSale(fundsPurchased, vm, importStatus, sharesFeeClass, import);
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Sälj", import, ex.Message);
            }
        }

        private static bool ValidateSaleInput(SharesPurchasedFundViewModel vm, bool import)
        {
            if (import)
                return vm.SaleFee > 0;

            return vm.SaleDateOfPurchase != DateTime.MinValue &&
                   vm.SaleHowMany > 0 &&
                   vm.SalePricePerFunds > 0 &&
                   vm.SaleFee > 0;
        }

        private async Task<string> HandleEntireShareSale(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, ISharesFeeClass sharesFeeClass, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("HandleEntireShareSale: db == null!");

                SharesSoldFunds soldFund = CreateSharesSoldFunds(fundsPurchased, vm, importStatus, true);

                await db.SharesSoldFunds.AddAsync(soldFund);
                await db.SaveChangesAsync();

                // Add fee to the fee table
                SharesFeeViewModel feeVm = ChangeFromToPurchasedToFeeViewModel(vm, soldFund.Fee, $"Avgiften för fonden: {vm.FundName}");

                if (string.IsNullOrEmpty(soldFund.DateOfSold))
                    return await HandleError(vm, "Sälj", import, "Finns inget sålt datum!");

                await sharesFeeClass.Add(feeVm, true);

                // Remove the purchased fund
                await Delete(fundsPurchased);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Sålt hela", import, $"Sälj: Ett fel uppstod vid försäljning av hela fonden. Felmeddelande: {ex.Message}");
            }
        }

        private async Task<string> HandlePartialShareSale(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, ISharesFeeClass sharesFeeClass, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("HandlePartialShareSale: db == null!");

                SharesSoldFunds soldFund = CreateSharesSoldFunds(fundsPurchased, vm, importStatus, false);

                await db.SharesSoldFunds.AddAsync(soldFund);
                await db.SaveChangesAsync();

                // Add fee to the fee table
                SharesFeeViewModel feeVm = ChangeFromToPurchasedToFeeViewModel(vm, soldFund.Fee, $"Avgift för sålda delar av fonden: {vm.FundName}");

                if (string.IsNullOrEmpty(soldFund.DateOfSold))
                    return await HandleError(vm, "Sälj", import, "Finns inget sålt datum!");

                await sharesFeeClass.Add(feeVm, import);

                string errorMessages = string.Empty;

                // Update the purchased fund
                errorMessages = await EditSell(fundsPurchased, vm, import);

                if (!string.IsNullOrEmpty(errorMessages))
                    return await HandleError(vm, "Uppdatera sålda fonden", import, errorMessages);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Sålt delar av", import, ex.Message);
            }
        }

        private static SharesSoldFunds CreateSharesSoldFunds(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, bool isEntireShare)
        {
            double soldAmount = vm.SalePricePerFunds * vm.SaleHowMany;
            double totalFee = isEntireShare ? fundsPurchased.Fee + vm.SaleFee : vm.SaleFee;

            return new SharesSoldFunds
            {
                DateOfPurchase = fundsPurchased.DateOfPurchase,
                DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                Amount = fundsPurchased.PricePerFunds * vm.SaleHowMany,
                Fee = totalFee,
                FundName = fundsPurchased.FundName,
                HowMany = vm.SaleHowMany,
                TypeOfFund = fundsPurchased.TypeOfFund,
                Currency = fundsPurchased.Currency,
                ISIN = fundsPurchased.ISIN,
                Account = fundsPurchased.Account,
                PricePerFunds = fundsPurchased.PricePerFunds,
                PricePerFundsSold = vm.SalePricePerFunds,
                AmountSold = soldAmount,
                Note = GenerateSaleNote(fundsPurchased, vm, importStatus, soldAmount, totalFee),
                MoneyProfitOrLoss = soldAmount - (fundsPurchased.PricePerFunds * vm.SaleHowMany),
                PercentProfitOrLoss = ConvertToPercentage((soldAmount / (fundsPurchased.PricePerFunds * vm.SaleHowMany)) - 1)
            };
        }

        private static string GenerateSaleNote(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, double soldAmount, double totalFee)
        {
            return $"{fundsPurchased.Note} \r\n\r\n" +
                   $"Sålt {(vm.SaleHowMany == fundsPurchased.HowMany ? "fonden" : "delar av fonden")}: {fundsPurchased.FundName} \r\n" +
                   $"Import: {importStatus} \r\n" +
                   $"Datum: {vm.SaleDateOfPurchase:yyyy-MM-dd} \r\n" +
                   $"Hur många: {vm.SaleHowMany} \r\n" +
                   $"Pris per st: {vm.SalePricePerFunds} \r\n" +
                   $"Summan: {soldAmount} \r\n" +
                   $"Avgift: {totalFee}";
        }

        // Removes portions of the purchased funds that are moved to sold funds
        private async Task<string> EditSell(SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("EditSell: db == null!");

                if (vm == null || model == null || db == null)
                    return await HandleError(vm, "Radera sålda", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

                string importTrue = import ? "Ja" : "Nej";


                // Update the model
                model.HowMany -= vm.SaleHowMany;
                model.Amount = model.HowMany * model.PricePerFunds;

                // Update the note
                if (!string.IsNullOrEmpty(model.Note))
                    model.Note = AppendSaleNote(model.Note, vm, importTrue);

                // Save changes asynchronously
                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Radera sålda", import, $"Ett fel uppstod: {ex.Message}");
            }
        }

        private static string AppendSaleNote(string existingNote, SharesPurchasedFundViewModel vm, string importTrue)
        {
            string saleNote = $"\r\n\r\nSålt delar av fonden {vm.FundName}: \r\n" +
                              $"Datum: {vm.SaleDateOfPurchase:yyyy-MM-dd} \r\n" +
                              $"Import: {importTrue} \r\n" +
                              $"Hur många: {vm.SaleHowMany} \r\n" +
                              $"Pris per st: {vm.SalePricePerFunds} \r\n" +
                              $"Summan: {vm.SaleHowMany * vm.SalePricePerFunds} \r\n" +
                              $"Courtage: {vm.SaleFee}";

            return string.IsNullOrEmpty(existingNote) ? saleNote : existingNote + saleNote;
        }

        public async Task<string> Delete(SharesPurchasedFunds model)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                if (model == null || model.SharesPurchasedFundId == 0 || db == null)
                    return await HandleError(null, "Ta bort såld", false, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

                db.SharesPurchasedFunds.Remove(model);
                await db.SaveChangesAsync().ConfigureAwait(false);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(null, "Ta bort såld", false, ex.Message);
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesPurchasedFundViewModel ChangeFromModelToViewModel(SharesPurchasedFunds model)
        {
            if (model == null)
                throw new Exception("ChangeFromModelToViewModel == null!");

            SharesPurchasedFundViewModel vm = _mapper.Map<SharesPurchasedFundViewModel>(model);

            vm.PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero);
            vm.Amount = double.Round(model.HowMany * model.PricePerFunds, 2, MidpointRounding.AwayFromZero);

            if (!string.IsNullOrEmpty(model.DateOfPurchase))
                vm.DateOfPurchase = ParseDate(model.DateOfPurchase);

            return vm;
        }

        private SharesPurchasedFunds ChangesFromViewModelToModel(SharesPurchasedFundViewModel vm)
        {
            if (vm == null)
                throw new Exception("ChangesFromViewModelToModel == null!");

            SharesPurchasedFunds model = _mapper.Map<SharesPurchasedFunds>(vm);

            model.PricePerFunds = double.Round(vm.PricePerFunds, 2, MidpointRounding.AwayFromZero);
            model.Amount = double.Round(vm.HowMany * vm.PricePerFunds, 2, MidpointRounding.AwayFromZero);

            if (vm.DateOfPurchase != DateTime.MinValue)
                model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");

            return model;
        }

        public SharesPurchasedFundViewModel ChangeFromImportSellToViewModel(SharesImports model)
        {
            if (model == null)
                throw new Exception("ChangeFromImportSellToViewModel == null!");

            if (string.IsNullOrEmpty(model.NumberOfSharesString) || string.IsNullOrEmpty(model.PricePerShareString) || string.IsNullOrEmpty(model.NumberOfSharesString))
                throw new Exception("Kolla tomma fält i import filen!");

            SharesPurchasedFundViewModel vm = new()
            {
                FundName = model.CompanyOrInformation,
                SaleHowMany = double.Parse(model.NumberOfSharesString),
                SalePricePerFunds = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                SaleFee = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.NumberOfSharesString) * double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero)
            };

            if (!string.IsNullOrEmpty(model.Date))
                vm.SaleDateOfPurchase = ParseDate(model.Date);

            return vm;
        }

        public SharesPurchasedFundViewModel ChangeFromImportAddToViewModel(SharesImports model)
        {
            if (model == null)
                throw new Exception("ChangeFromImportAddToViewModel == null!");

            if (string.IsNullOrEmpty(model.NumberOfSharesString) || string.IsNullOrEmpty(model.PricePerShareString) || string.IsNullOrEmpty(model.NumberOfSharesString))
                throw new Exception("Kolla tomma fält i import filen!");

            SharesPurchasedFundViewModel vm = new()
            {
                FundName = model.CompanyOrInformation,
                HowMany = double.Parse(model.NumberOfSharesString),
                PricePerFunds = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                Fee = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.NumberOfSharesString) * double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero)
            };

            if (!string.IsNullOrEmpty(model.Date))
                vm.DateOfPurchase = ParseDate(model.Date);

            return vm;
        }

        private static SharesFeeViewModel ChangeFromToPurchasedToFeeViewModel(SharesPurchasedFundViewModel vm, double tax, string note)
        {
            SharesFeeViewModel fee = new()
            {
                Date = DateTime.Now,
                CompanyOrInformation = vm.FundName,
                Tax = tax,
                Note = note,

                // For error information
                DateOfFee = vm.SaleDateOfPurchase,
                Account = vm.Account,
                TypeOfTransaction = "Sälj fond",
                ISIN = vm.ISIN
            };

            return fee;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private async Task<string> HandleError(SharesPurchasedFundViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import && vm != null)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesPurchasedFundViewModel? vm, string type, bool import, string errorMessage)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("ErrorHandling: db == null!");
                
                if (vm == null)
                    throw new Exception("ErrorHandling: SharesPurchasedFundViewModel == null!");

                DateTime date = DateTime.Now;
                string importTrue = import ? "Ja" : "Nej";


                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.FundName,
                    TypeOfTransaction = type + " fond",
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} fond: " +
                       $"\r\nKöp datum: {vm.DateOfPurchase.ToString()[..10]} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nId: {vm.SharesPurchasedFundId} " +
                       $"\r\nISIN: {vm.ISIN}."
                };

                await db.SharesErrorHandlings.AddAsync(sharesErrorHandling);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fonder: Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }
    }
}