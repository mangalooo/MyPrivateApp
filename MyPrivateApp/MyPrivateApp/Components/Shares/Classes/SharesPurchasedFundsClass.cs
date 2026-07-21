
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using System.Globalization;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedFundsClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesPurchasedFundsClass> logger) : ISharesPurchasedFundsClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesPurchasedFundsClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<string> Add(SharesPurchasedFundViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Add: db == null!");

                if (vm == null)
                    return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret!");

                List<string> missing = [];

                if (string.IsNullOrEmpty(vm.FundName)) missing.Add("Fond namn");
                if (string.IsNullOrEmpty(vm.ISIN)) missing.Add("ISIN");
                if (vm.DateOfPurchase == DateTime.MinValue) missing.Add("Inköpsdatum");
                if (vm.HowMany <= 0) missing.Add("Antal");
                if (vm.PricePerFunds <= 0) missing.Add("Pris per fonddel");

                if (missing.Count > 0)
                    return await HandleError(vm, "Köpt", import, $"Du måste fylla i fälten: {string.Join(", ", missing)}!");

                SharesPurchasedFunds model = ChangesFromViewModelToModel(vm);

                string importTrue = import ? "Ja" : "Nej";

                model.Note += $"Köper:" +
                              $"\r\nBolag: {model.FundName} aktier" +
                              $"\r\nISIN: {vm.ISIN}" +
                              $"\r\nHur många: {model.HowMany}" +
                              $"\r\nPris per st: {model.PricePerFunds}" +
                              $"\r\nVärdet: {model.Amount}" +
                              $"\r\nImport: {importTrue}";

                if (!string.IsNullOrEmpty(model.DateOfPurchase))
                    model.Note += $"\r\nDatum: {model.DateOfPurchase.ToString()[..10]}";

                await db.SharesPurchasedFunds.AddAsync(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
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
                using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Edit: db == null!");

                if (vm == null || db == null || vm.SharesPurchasedFundId <= 0)
                    return "Hittar ingen data från formuläret eller databasen!";

                if (vm.DateOfPurchase == DateTime.MinValue || string.IsNullOrEmpty(vm.FundName) || string.IsNullOrEmpty(vm.ISIN)
                     || vm.HowMany <= 0 || vm.PricePerFunds <= 0)
                    return "Du måste fylla i fälten: Fond namn, ISIN, Inköpsdatum, Antal, Pris per fond del!";

                SharesPurchasedFunds? model = await db.SharesPurchasedFunds.FirstOrDefaultAsync(r => r.ISIN == vm.ISIN)
                    ?? throw new Exception("Den köpte fonden hittades inte i databasen!");

                if (model == null)
                    return "Hittar inte den köpa fonden i databasen!";

                EditModel(model, vm);

                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
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
                using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("AddMore: db == null!");

                if (vm == null || db == null)
                    return await HandleError(vm, "Köpt mera", import, "Ingen kontakt med VM eller DB för fonden!");

                if (!import && (vm.MoreDateOfPurchase == DateTime.MinValue || vm.MoreHowMany <= 0 || vm.MorePricePerFunds <= 0))
                    return "Du måste fylla i fälten: Köp mer: Datum, Köp mer: Antal och Köp mer: Pris per fond del!";


                if (string.IsNullOrEmpty(vm.ISIN))
                    return await HandleError(vm, "Köpt mera", import, "Finns inget ISIN till fonden!");

                SharesPurchasedFunds? model = await db.SharesPurchasedFunds.FirstOrDefaultAsync(r => r.ISIN == vm.ISIN)
                    ?? throw new Exception("Den köpte fonden hittades inte i databasen!");

                if (model == null)
                    return await HandleError(vm, "Köpt mera", import, "Hittar inte fonden i databasen!");

                db.Attach(model);

                UpdateModelWithAdditionalPurchase(model, vm, import);
                
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt mera", import, ex.Message);
            }
        }

        public async Task<string> Delete(SharesPurchasedFunds model)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext()
                    ?? throw new Exception("Delete: db == null!");

                if (model == null || model.SharesPurchasedFundId == 0 || db == null)
                    return await HandleError(null, "Ta bort såld", false, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

                db.SharesPurchasedFunds.Remove(model);
                await db.SaveChangesAsync().ConfigureAwait(false);
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(null, "Ta bort såld", false, ex.Message);
            }
        }

        private static void UpdateModelWithAdditionalPurchase(SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import)
        {
            if (import)
            {
                model.HowMany += vm.HowMany;
                model.Amount += vm.HowMany * vm.PricePerFunds;
                model.Note += GenerateNoteForAdditionalPurchase(vm, import, model.Amount);
            }
            else
            {
                model.HowMany += vm.MoreHowMany;
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

            return $"\r\n\r\nKöper mer fondandelar av {vm.FundName}: \r\n" +
                   $"Import: {importTrue} \r\n" +
                   $"Datum: {date} \r\n" +
                   $"Hur många: {Math.Round(howMany, 2, MidpointRounding.AwayFromZero)} \r\n" +
                   $"Pris per st: {Math.Round(pricePerFunds, 2, MidpointRounding.AwayFromZero)} \r\n" +
                   $"Summan: {Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero)}";
        }

        // Selling all or part of the fund
        public async Task<string> Sell(SharesPurchasedFundViewModel vm, bool import)
        {
            if (vm == null || string.IsNullOrEmpty(vm.ISIN))
                return await HandleError(vm, "Sälj", import, "Formuläret eller ISIN är tomt!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext()
                ?? throw new Exception("Sell: db == null!");

            SharesPurchasedFunds? fundsPurchased = await db.SharesPurchasedFunds.FirstOrDefaultAsync(r => r.ISIN == vm.ISIN)
                ?? throw new Exception("Den köpte fonden hittades inte i databasen!");

            db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

            if (fundsPurchased == null)
                return await HandleError(vm, "Sälj", import, "Fonden hittades inte i databasen!");

            vm.SharesPurchasedFundId = fundsPurchased.SharesPurchasedFundId;

            if (!ValidateSaleInput(vm/*, import*/))
                return await HandleError(vm, "Sälj", import, "Du måste fylla i alla obligatoriska fält korrekt!");

            string importStatus = import ? "Ja" : "Nej";

            try
            {
                if (fundsPurchased.HowMany == vm.SaleHowMany)
                    // Handle entire share sale
                    return await HandleEntireShareSale(fundsPurchased, vm, importStatus, import);

                else
                    // Handle partial share sale
                    return await HandlePartialShareSale(fundsPurchased, vm, importStatus, import);
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Sälj", import, ex.Message);
            }
        }

        private static bool ValidateSaleInput(SharesPurchasedFundViewModel vm/*, bool import*/)
        {
            return vm.SaleDateOfPurchase != DateTime.MinValue &&
                   vm.SaleHowMany > 0 &&
                   vm.SalePricePerFunds > 0;
        }

        private async Task<string> HandleEntireShareSale(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("HandleEntireShareSale: db == null!");

                SharesSoldFunds soldFund = CreateSharesSoldFunds(fundsPurchased, vm, importStatus, true);

                await db.SharesSoldFunds.AddAsync(soldFund);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

                // Remove the purchased fund
                await Delete(fundsPurchased);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Sålt hela", import, $"Sälj: Ett fel uppstod vid försäljning av hela fonden. Felmeddelande: {ex.Message}");
            }
        }

        private async Task<string> HandlePartialShareSale(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("HandlePartialShareSale: db == null!");

                SharesSoldFunds soldFund = CreateSharesSoldFunds(fundsPurchased, vm, importStatus, false);

                await db.SharesSoldFunds.AddAsync(soldFund);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues

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
            double soldAmount = Math.Round(vm.SalePricePerFunds * vm.SaleHowMany, 2, MidpointRounding.AwayFromZero);

            return new SharesSoldFunds
            {
                DateOfPurchase = fundsPurchased.DateOfPurchase,
                DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                Amount = Math.Round(fundsPurchased.PricePerFunds * vm.SaleHowMany, 2, MidpointRounding.AwayFromZero),
                FundName = fundsPurchased.FundName,
                HowMany = vm.SaleHowMany,
                TypeOfFund = fundsPurchased.TypeOfFund,
                Currency = fundsPurchased.Currency,
                ISIN = fundsPurchased.ISIN,
                Account = fundsPurchased.Account,
                PricePerFunds = Math.Round(fundsPurchased.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                PricePerFundsSold = Math.Round(vm.SalePricePerFunds, 2, MidpointRounding.AwayFromZero),
                AmountSold = Math.Round(double.Parse(Math.Round(soldAmount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00")), 2, MidpointRounding.AwayFromZero),
                Note = GenerateSaleNote(fundsPurchased, vm, importStatus, soldAmount),
                MoneyProfitOrLoss = Math.Round(soldAmount - (fundsPurchased.PricePerFunds * vm.SaleHowMany), 2, MidpointRounding.AwayFromZero),
                PercentProfitOrLoss = ConvertToPercentage((soldAmount / (fundsPurchased.PricePerFunds * vm.SaleHowMany)) - 1)
            };
        }

        private static string GenerateSaleNote(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, double soldAmount)
        {
            return $"{fundsPurchased.Note} \r\n\r\n" +
                   $"Sålt {(vm.SaleHowMany == fundsPurchased.HowMany ? "fonden" : "delar av fonden")}: {fundsPurchased.FundName} \r\n" +
                   $"Import: {importStatus} \r\n" +
                   $"Datum: {vm.SaleDateOfPurchase:yyyy-MM-dd} \r\n" +
                   $"Hur många: {vm.SaleHowMany} \r\n" +
                   $"Pris per st: {vm.SalePricePerFunds} \r\n" +
                   $"Summan: {soldAmount:#,##0.00} \r\n";
        }

        // Removes portions of the purchased funds that are moved to sold funds
        private async Task<string> EditSell(SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("EditSell: db == null!");

                if (vm == null || model == null || db == null)
                    return await HandleError(vm, "Radera sålda", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

                string importTrue = import ? "Ja" : "Nej";

                // Update the model
                model.HowMany -= vm.SaleHowMany;
                model.Amount = model.HowMany * model.PricePerFunds;

                // Update the note
                if (!string.IsNullOrEmpty(model.Note))
                    model.Note = AppendSaleNote(model.Note, vm, importTrue);

                db.SharesPurchasedFunds.Update(model);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
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
                              $"Summan: {vm.SaleHowMany * vm.SalePricePerFunds} \r\n";

            return string.IsNullOrEmpty(existingNote) ? saleNote : existingNote + saleNote;
        }

        private static DateTime ParseDate(string? date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return DateTime.MinValue;

            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;
        }

        public SharesPurchasedFundViewModel ChangeFromModelToViewModel(SharesPurchasedFunds model)
        {
            if (model == null)
                throw new Exception("ChangeFromModelToViewModel == null!");

            return new SharesPurchasedFundViewModel
            {
                SharesPurchasedFundId = model.SharesPurchasedFundId,
                DateOfPurchase = ParseDate(model.DateOfPurchase),
                FundName = model.FundName,
                HowMany = model.HowMany,
                PricePerFunds = model.PricePerFunds,
                Fee = model.Fee,
                Amount = model.Amount,
                TypeOfFund = model.TypeOfFund,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Note = model.Note
            };
        }

        private static SharesPurchasedFunds ChangesFromViewModelToModel(SharesPurchasedFundViewModel vm)
        {
            if (vm == null)
                throw new Exception("ChangesFromViewModelToModel == null!");

            return new SharesPurchasedFunds
            {
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                FundName = vm.FundName,
                HowMany = Math.Round(vm.HowMany, 2, MidpointRounding.AwayFromZero),
                PricePerFunds = Math.Round(vm.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                Fee = vm.Fee,
                Amount = Math.Round(double.Parse(Math.Round(vm.HowMany * vm.PricePerFunds, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00")), 2, MidpointRounding.AwayFromZero),
                TypeOfFund = vm.TypeOfFund,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Account = vm.Account,
                Note = vm.Note
            };
        }

        private static void EditModel(SharesPurchasedFunds model, SharesPurchasedFundViewModel vm)
        {
            model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
            model.FundName = vm.FundName;
            model.HowMany = vm.HowMany;
            model.PricePerFunds = vm.PricePerFunds;
            model.Fee = vm.Fee;
            model.Amount = vm.Amount;
            model.TypeOfFund = vm.TypeOfFund;
            model.Currency = vm.Currency;
            model.ISIN = vm.ISIN;
            model.Account = vm.Account;
            model.Note = vm.Note;
        }

        private static readonly CultureInfo Sv = CultureInfo.GetCultureInfo("sv-SE");

        private static double ParseFeeOrZero(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0d;
            if (double.TryParse(s.Trim(), NumberStyles.Any, Sv, out var v)) return v;
            throw new FormatException($"Invalid fee: '{s}'");
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
                SaleHowMany = Math.Round(double.Parse(model.NumberOfSharesString), 2, MidpointRounding.AwayFromZero),
                SalePricePerFunds = Math.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                SaleFee = Math.Round(ParseFeeOrZero(model.BrokerageString), 2, MidpointRounding.AwayFromZero),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Parse(Math.Round(double.Parse(model.NumberOfSharesString) * double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"))
            };

            if (!string.IsNullOrEmpty(model.Date))
                vm.SaleDateOfPurchase = ParseDate(model.Date);

            return vm;
        }

        private static double ParseDoubleOrThrow(string? s, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new FormatException($"{fieldName} is empty.");

            if (double.TryParse(s.Trim(), NumberStyles.Any, Sv, out var v))
                return v;

            throw new FormatException($"{fieldName} has invalid number: '{s}'.");
        }

        private static double ParseDoubleOrDefault(string? s, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            return double.TryParse(s.Trim(), NumberStyles.Any, Sv, out var v) ? v : defaultValue;
        }

        public SharesPurchasedFundViewModel ChangeFromImportAddToViewModel(SharesImports model)
        {
            if (string.IsNullOrWhiteSpace(model.NumberOfSharesString) ||
                string.IsNullOrWhiteSpace(model.PricePerShareString))
                throw new Exception("Kolla tomma fält i import filen!");

            double price = ParseDoubleOrThrow(model.PricePerShareString, nameof(model.PricePerShareString));
            double fee = ParseDoubleOrDefault(model.BrokerageString, 0); // blank => 0

            return new SharesPurchasedFundViewModel
            {
                DateOfPurchase = ParseDate(model.Date),
                FundName = model.CompanyOrInformation,
                HowMany = Math.Round(double.Parse(model.NumberOfSharesString), 2, MidpointRounding.AwayFromZero),
                PricePerFunds = Math.Round(price, 2, MidpointRounding.AwayFromZero),
                Fee = fee,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = Math.Round((double)ParseDoubleOrThrow(model.NumberOfSharesString, nameof(model.NumberOfSharesString)) * price, 2, MidpointRounding.AwayFromZero)
            };
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
                using ApplicationDbContext db = _dbFactory.CreateDbContext() 
                    ?? throw new Exception("ErrorHandling: db == null!");
                
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
                db.ChangeTracker.Clear(); // Clear the change tracker to avoid tracking issues
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fonder: Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }
    }
}