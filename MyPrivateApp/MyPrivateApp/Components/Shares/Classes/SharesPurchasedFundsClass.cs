
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using System.Threading.Tasks;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedFundsClass(ApplicationDbContext db, ILogger<SharesPurchasedClass> logger, IMapper mapper) : ISharesPurchasedFundsClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesPurchasedClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesPurchasedFunds?> Get(string ISIN)
        {
            if (string.IsNullOrEmpty(ISIN))
                throw new ArgumentNullException(nameof(ISIN));

            return await _db.SharesPurchasedFunds.FirstOrDefaultAsync(r => r.ISIN == ISIN)
                ?? throw new Exception("Den köpte fonden hittades inte i databasen!");
        }

        public async Task<string> Add(SharesPurchasedFundViewModel vm, bool import)
        {
            if (vm == null || _db == null)
                return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret eller databasen!");

            if (vm.DateOfPurchase == DateTime.MinValue && string.IsNullOrEmpty(vm.FundName) &&
                string.IsNullOrEmpty(vm.ISIN) && vm.HowMany <= 0 && string.IsNullOrEmpty(vm.PricePerFunds) && vm.Fee <= 0)
                return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Fond namn, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!");

            try
            {
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

                await _db.SharesPurchasedFunds.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesPurchasedFundViewModel vm)
        {
            if (vm == null || _db == null || vm.SharesPurchasedFundId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            if (vm.DateOfPurchase == DateTime.MinValue || string.IsNullOrEmpty(vm.FundName) || string.IsNullOrEmpty(vm.ISIN)
                 || vm.HowMany <= 0 || string.IsNullOrEmpty(vm.PricePerFunds) || vm.Fee <= 0)
                return "Du måste fylla i fälten: Fond namn, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!";

            try
            {
                SharesPurchasedFunds? model = await Get(vm.ISIN);

                if (model == null)
                    return "Hittar inte den köpa fonden i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;

            }
            catch (Exception ex)
            {
                return $"Ändra köpt fond. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> AddMore(SharesPurchasedFundViewModel vm, bool import)
        {
            if (!ValidateAddMoreInput(vm, import))
                return "Du måste fylla i fälten: Köp mer: Datum, Köp mer: Antal och Köp mer: Pris per fond del!";

            try
            {
                if (string.IsNullOrEmpty(vm.ISIN))
                    return "Finns inget ISIN!";

                var model = await Get(vm.ISIN);
                if (model == null)
                    return HandleNotFoundError(import, "Hittar inte fonden i databasen!");

                UpdateModelWithAdditionalPurchase(model, vm, import);

                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                await HandleError(vm, "Köpt mera", import, ex.Message);
                return $"Ett fel uppstod: {ex.Message}";
            }
        }

        private bool ValidateAddMoreInput(SharesPurchasedFundViewModel vm, bool import)
        {
            if (vm == null || string.IsNullOrEmpty(vm.ISIN) || _db == null)
                return false;

            if (!import && (vm.MoreDateOfPurchase == DateTime.MinValue || vm.MoreHowMany <= 0 || vm.MorePricePerFunds <= 0))
                return false;

            return true;
        }

        private static void UpdateModelWithAdditionalPurchase(SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import)
        {
            if (import)
            {
                model.HowMany += vm.HowMany;
                model.Fee += vm.Fee;
                model.Amount += vm.HowMany * double.Parse(vm.PricePerFunds);
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
            double pricePerFunds = import ? double.Parse(vm.PricePerFunds) : vm.MorePricePerFunds;
            double fee = import ? vm.Fee : vm.MoreFee;

            return $"\r\n\r\nKöper mer fond delar för {vm.FundName}: \r\n" +
                   $"Import: {importTrue} \r\n" +
                   $"Datum: {date} \r\n" +
                   $"Hur många: {howMany} \r\n" +
                   $"Pris per st: {pricePerFunds} \r\n" +
                   $"Summan: {totalAmount} \r\n" +
                   $"Courtage: {fee}";
        }

        private string HandleNotFoundError(bool import, string message)
        {
            if (import)
            {
                _logger.LogError(message);
                return string.Empty;
            }

            return message;
        }

        // Selling all or part of the fund
        public async Task<string> Sell(SharesPurchasedFundViewModel vm, bool import, ISharesFeeClass sharesFeeClass)
        {
            if (vm == null || string.IsNullOrEmpty(vm.ISIN))
                return "Formuläret eller ISIN är tomt!";

            SharesPurchasedFunds fundsPurchased = await Get(vm.ISIN);

            if (fundsPurchased == null)
                return "Fonden hittades inte i databasen!";

            vm.SharesPurchasedFundId = fundsPurchased.SharesPurchasedFundId;

            if (!ValidateSaleInput(vm, import))
                return "Du måste fylla i alla obligatoriska fält korrekt!";

            string importStatus = import ? "Ja" : "Nej";

            try
            {
                if (fundsPurchased.HowMany == vm.SaleHowMany)
                    // Handle entire share sale
                    return await HandleEntireShareSale(fundsPurchased, vm, importStatus, sharesFeeClass);

                else
                    // Handle partial share sale
                    return await HandlePartialShareSale(fundsPurchased, vm, importStatus, sharesFeeClass);
            }
            catch (Exception ex)
            {
                await HandleError(vm, "Sälj", import, ex.Message);
                return $"Ett fel uppstod: {ex.Message}";
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

        private async Task<string> HandleEntireShareSale(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, ISharesFeeClass sharesFeeClass)
        {
            var soldFund = CreateSharesSoldFunds(fundsPurchased, vm, importStatus, true);

            try
            {
                await _db.SharesSoldFunds.AddAsync(soldFund);
                await db.SaveChangesAsync();

                // Add fee to the fee table
                var feeVm = ChangeFromToPurchasedToFeeViewModel(vm, soldFund.Fee, $"Avgiften för fonden: {vm.FundName}");

                if (string.IsNullOrEmpty(soldFund.DateOfSold))
                    return "Finns inget sålt datum!";

                await sharesFeeClass.Add(feeVm, true, soldFund.DateOfSold);

                // Remove the purchased fund
                Delete(db, fundsPurchased, vm, true);

                return string.Empty;
            }
            catch (Exception ex)
            {
                await HandleError(vm, "Såld lägg till", true, ex.Message);
                return $"Sälj: Ett fel uppstod vid försäljning av hela fonden: {ex.Message}";
            }
        }

        private async Task<string> HandlePartialShareSale(SharesPurchasedFunds fundsPurchased, SharesPurchasedFundViewModel vm, string importStatus, ISharesFeeClass sharesFeeClass)
        {
            var soldFund = CreateSharesSoldFunds(fundsPurchased, vm, importStatus, false);

            try
            {
                await _db.SharesSoldFunds.AddAsync(soldFund);
                await db.SaveChangesAsync();

                // Add fee to the fee table
                SharesFeeViewModel feeVm = ChangeFromToPurchasedToFeeViewModel(vm, soldFund.Fee, $"Avgift för sålda delar av fonden: {vm.FundName}");

                if (string.IsNullOrEmpty(soldFund.DateOfSold))
                    return "Finns inget sålt datum!";

                await sharesFeeClass.Add(feeVm, false, soldFund.DateOfSold);

                // Update the purchased fund
                await EditSell(fundsPurchased, vm, false);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Sålt delar av", false, ex.Message);
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
        private async Task EditSell(SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import)
        {
            if (vm == null || model == null || db == null)
            {
                await HandleError(vm, "Radera sålda", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                return;
            }

            string importTrue = import ? "Ja" : "Nej";

            try
            {
                // Update the model
                model.HowMany -= vm.SaleHowMany;
                model.Amount = model.HowMany * model.PricePerFunds;

                // Update the note
                if (!string.IsNullOrEmpty(model.Note))
                    model.Note = AppendSaleNote(model.Note, vm, importTrue);

                // Save changes asynchronously
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await HandleError(vm, "Radera sålda", import, $"Ett fel uppstod: {ex.Message}");
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
            if (model == null || model.SharesPurchasedFundId == 0 || _db == null)
                return await HandleError(null, "Ta bort såld", false, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

            try
            {
                _db.SharesPurchasedFunds.Remove(model);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return await HandleError(null, "Ta bort såld", false, ex.Message);
            }

            return string.Empty;
        }

        public SharesPurchasedFundViewModel ChangeFromModelToViewModel(SharesPurchasedFunds model)
        {
            DateTime date = DateTime.Parse(model.DateOfPurchase);

            SharesPurchasedFundViewModel vm = new()
            {
                SharesPurchasedFundId = model.SharesPurchasedFundId,
                DateOfPurchase = date,
                FundName = model.FundName,
                HowMany = model.HowMany,
                PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Fee = model.Fee,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                TypeOfFund = model.TypeOfFund,
                Note = model.Note
            };

            return vm;
        }

        public SharesPurchasedFundViewModel ChangeFromImportSellToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesPurchasedFundViewModel vm = new()
            {
                SaleDateOfPurchase = date,
                FundName = model.CompanyOrInformation,
                SaleHowMany = double.Round(double.Parse(model.NumberOfSharesString), 2, MidpointRounding.AwayFromZero),
                SalePricePerFunds = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                SaleFee = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
            };

            return vm;
        }

        public SharesPurchasedFundViewModel ChangeFromImportAddToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesPurchasedFundViewModel vm = new()
            {
                DateOfPurchase = date,
                FundName = model.CompanyOrInformation,
                HowMany = double.Parse(model.NumberOfSharesString),
                PricePerFunds = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Fee = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
            };

            return vm;
        }

        private static SharesPurchasedFunds ChangesFromViewModelToModel(SharesPurchasedFundViewModel vm)
        {
            SharesPurchasedFunds sharesPurchasedFunds = new()
            {
                SharesPurchasedFundId = vm.SharesPurchasedFundId,
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                FundName = vm.FundName,
                HowMany = vm.HowMany,
                PricePerFunds = double.Round(double.Parse(vm.PricePerFunds), 2, MidpointRounding.AwayFromZero),
                Fee = vm.Fee,
                Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerFunds), 2, MidpointRounding.AwayFromZero),
                ISIN = vm.ISIN,
                Currency = vm.Currency,
                Account = vm.Account,
                TypeOfFund = vm.TypeOfFund,
                Note = vm.Note
            };

            return sharesPurchasedFunds;
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
            ArgumentNullException.ThrowIfNull(vm);

            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.FundName,
                    TypeOfTransaction = type + " aktie",
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} fond: " +
                       $"\r\nKöp datum: {vm.DateOfPurchase} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nId: {vm.SharesPurchasedFundId} " +
                       $"\r\nISIN: {vm.ISIN}."
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