
using AutoMapper;
using MyPrivateApp.Data;
using Microsoft.EntityFrameworkCore;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;

using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedClass(ApplicationDbContext db, ILogger<SharesPurchasedClass> logger, IMapper mapper) : ISharesPurchasedClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<SharesPurchasedClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        private async Task<SharesPurchaseds?> Get(string ISIN)
        {
            if (string.IsNullOrEmpty(ISIN))
                throw new ArgumentNullException(nameof(ISIN));

            return await _db.SharesPurchaseds.FirstOrDefaultAsync(r => r.ISIN == ISIN)
                ?? throw new Exception("Den köpte aktien hittades inte i databasen!");
        }

        public async Task<string> Add(SharesPurchasedViewModel vm, bool import)
        {
            if (vm == null || _db == null)
                return await HandleError(vm, "Köpt", import, "Hittar ingen data från formuläret eller databasen!");

            if (vm.DateOfPurchase == DateTime.MinValue && string.IsNullOrEmpty(vm.CompanyName) &&
                string.IsNullOrEmpty(vm.ISIN) && vm.HowMany <= 0 && string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage <= 0)
                return await HandleError(vm, "Köpt", import, "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!");

            try
            {
                SharesPurchaseds model = ChangesFromViewModelToModel(vm);
                string importTrue = import ? "Ja" : "Nej";
                model.Note += $"Köper:" +
                              $"\r\nBolag: {model.CompanyName} aktier" +
                              $"\r\nISIN: {vm.ISIN} " +
                              $"\r\nDatum: {model.DateOfPurchase.ToString()[..10]}" +
                              $"\r\nHur många: {model.HowMany} " +
                              $"\r\nPris per st: {model.PricePerShares}" +
                              $"\r\nVärdet: {model.Amount}" +
                              $"\r\nCourtage: {model.Brokerage}. " +
                              $"\r\nImport: {importTrue} ";

                await _db.SharesPurchaseds.AddAsync(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt", import, ex.Message);
            }
        }

        public async Task<string> Edit(SharesPurchasedViewModel vm)
        {
            if (vm == null || _db == null || vm.SharesPurchasedId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            if (vm.DateOfPurchase == DateTime.MinValue || string.IsNullOrEmpty(vm.CompanyName) || string.IsNullOrEmpty(vm.ISIN)
                 || vm.HowMany <= 0 || string.IsNullOrEmpty(vm.PricePerShares) || vm.Brokerage <= 0)
                return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!";

            try
            {
                SharesPurchaseds? model = await Get(vm.ISIN);

                if (model == null)
                    return "Hittar inte den köpa aktien i databasen!";

                _mapper.Map(vm, model);
                await _db.SaveChangesAsync();
                return string.Empty;

            }
            catch (Exception ex)
            {
                return $"Ändra köpt aktie. Felmeddelande: {ex.Message}";
            }
        }

        public async Task<string> AddMore(SharesPurchasedViewModel vm, bool import)
        {
            try
            {
                if (vm == null || string.IsNullOrEmpty(vm.ISIN) || _db == null)
                    return await HandleError(vm, "Köpt mera", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

                if (!import && (vm.MoreDateOfPurchase == DateTime.MinValue || vm.MoreHowMany == 0 || vm.MorePricePerShares == 0 || vm.MoreBrokerage == 0))
                    return "Du måste fylla i fälten: Köp mer: Datum, Köp mer: Antal, Köp mer: Pris per aktie, Köp mer: Courage!";

                SharesPurchaseds? model = await Get(vm.ISIN);
                if (model == null)
                    return await HandleError(vm, "Köpt mera", import, "Hittar inte aktien i databasen!");

                UpdateModelWithAdditionalPurchase(model, vm, import);

                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Köpt mera", import, ex.Message);
            }
        }

        private static void UpdateModelWithAdditionalPurchase(SharesPurchaseds model, SharesPurchasedViewModel vm, bool import)
        {
            if (import)
            {
                model.HowMany += vm.HowMany;
                model.Brokerage += vm.Brokerage;
                model.Amount += vm.HowMany * double.Parse(vm.PricePerShares!);
                model.Note += GenerateNoteForAdditionalPurchase(vm, import, model.Amount);
            }
            else
            {
                model.HowMany += vm.MoreHowMany;
                model.Brokerage += vm.MoreBrokerage;
                model.Amount += vm.MoreHowMany * vm.MorePricePerShares;
                model.Note += GenerateNoteForAdditionalPurchase(vm, import, model.Amount);
            }

            model.PricePerShares = model.Amount / model.HowMany;
        }

        private static string GenerateNoteForAdditionalPurchase(SharesPurchasedViewModel vm, bool import, double totalAmount)
        {
            string importTrue = import ? "Ja" : "Nej";

            return $"\r\n\r\nKöper mer:" +
                   $"\r\nBolag: {vm.CompanyName}" +
                   $"\r\nDatum: {(import ? vm.DateOfPurchase : vm.MoreDateOfPurchase).ToString("yyyy-MM-dd")}" +
                   $"\r\nHur många: {(import ? vm.HowMany : vm.MoreHowMany)}" +
                   $"\r\nPris per st: {(import ? vm.PricePerShares : vm.MorePricePerShares)}" +
                   $"\r\nInköpsvärdet: {(import ? vm.HowMany * double.Parse(vm.PricePerShares!) : vm.MoreHowMany * vm.MorePricePerShares)}" +
                   $"\r\nTotala värdet: {totalAmount}" +
                   $"\r\nCourtage: {(import ? vm.Brokerage : vm.MoreBrokerage)}" +
                   $"\r\nImport: {importTrue}";
        }

        // Selling all or part of the share
        public async Task<string> Sell(SharesPurchasedViewModel vm, bool import, ISharesFeeClass sharesFeeClass)
        {
            if (vm == null || string.IsNullOrEmpty(vm.ISIN))
                return await HandleError(vm, "Sälj", import, "Hittar ingen data från formuläret eller ISIN är tomt!");

            SharesPurchaseds? model = await Get(vm.ISIN);
            if (model == null || model.SharesPurchasedId != 0)
                return await HandleError(vm, "Sälj", import, "Aktien hittades inte i databasen!");

            if (!import && (vm.SaleDateOfPurchase == DateTime.MinValue || vm.SaleHowMany <= 0 || vm.SalePricePerShares <= 0 || vm.SaleBrokerage <= 0))
                return "Du måste fylla i fälten: Sälj datum, Sälj antal, Sälj pris per aktie, Sälj courage!";

            if (import && vm.SaleBrokerage == 0)
                return "Du får inte sälja aktien utan courage avgift!";

            string importTrue = import ? "Ja" : "Nej";
            vm.SharesPurchasedId = model.SharesPurchasedId;

            try
            {
                if (model.HowMany == vm.SaleHowMany)
                    await SellEntireShare(vm, model, importTrue, sharesFeeClass);
                else
                    await SellPartialShare(vm, model, importTrue, sharesFeeClass);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Sälj", import, ex.Message);
            }
        }

        private async Task SellEntireShare(SharesPurchasedViewModel vm, SharesPurchaseds purchasedsModel, string importTrue, ISharesFeeClass sharesFeeClass)
        {
            SharesSolds soldModel = CreateSharesSolds(vm, purchasedsModel, importTrue, true);

            soldModel.MoneyProfitOrLoss = soldModel.AmountSold - soldModel.Amount;
            soldModel.PercentProfitOrLoss = ConvertToPercentage((soldModel.AmountSold / soldModel.Amount) - 1);

            await _db.SharesSolds.AddAsync(soldModel);
            await _db.SaveChangesAsync();

            var feeVM = ChangeFromToPurchasedToFeeViewModel(vm, soldModel.Brokerage, $"Courtage för aktien: \r\nBolag: {vm.CompanyName} \r\nISIN: {vm.ISIN}");
            await sharesFeeClass.Add(feeVM, true, soldModel.DateOfSold);

            await Delete(purchasedsModel, vm, true);
        }

        private async Task SellPartialShare(SharesPurchasedViewModel vm, SharesPurchaseds sharesPurchaseds, string importTrue, ISharesFeeClass sharesFeeClass)
        {
            SharesSolds shares = CreateSharesSolds(vm, sharesPurchaseds, importTrue, false);

            shares.MoneyProfitOrLoss = shares.AmountSold - shares.Amount;
            shares.PercentProfitOrLoss = ConvertToPercentage((shares.AmountSold / shares.Amount) - 1);

            await _db.SharesSolds.AddAsync(shares);
            await _db.SaveChangesAsync();

            SharesFeeViewModel feeVM = ChangeFromToPurchasedToFeeViewModel
                (vm, shares.Brokerage, $"Courtage för sålda delar av aktien: \r\nBolag: {vm.CompanyName} \r\nISIN: {vm.ISIN}");

            await sharesFeeClass.Add(feeVM, false, shares.DateOfSold);

            await EditSell(sharesPurchaseds, vm, true);
        }

        private static SharesSolds CreateSharesSolds(SharesPurchasedViewModel vm, SharesPurchaseds model, string importTrue, bool isEntireShare)
        {
            return new SharesSolds
            {
                DateOfPurchase = model.DateOfPurchase,
                DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                Amount = model.PricePerShares * vm.SaleHowMany,
                Brokerage = isEntireShare ? model.Brokerage + vm.SaleBrokerage : vm.SaleBrokerage,
                CompanyName = model.CompanyName,
                HowMany = vm.SaleHowMany,
                TypeOfShares = model.TypeOfShares,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                PricePerShares = model.PricePerShares,
                PricePerSharesSold = vm.SalePricePerShares,
                AmountSold = vm.SalePricePerShares * vm.SaleHowMany,
                Note = $"{model.Note} " +
                       $"\r\n\r\n{(isEntireShare ? "Såld" : "Sålt delar")}: " +
                       $"\r\nBolag: {model.CompanyName}" +
                       $"\r\nDatum: {vm.SaleDateOfPurchase:yyyy-MM-dd}" +
                       $"\r\nHur många: {vm.SaleHowMany}" +
                       $"\r\nPris per st: {vm.SalePricePerShares} " +
                       $"\r\nSälj värdet: {vm.SaleHowMany * vm.SalePricePerShares}" +
                       $"\r\nCourtage: {vm.SaleBrokerage} " +
                       $"\r\nImport: {importTrue} "
            };
        }

        // Removes portions of the purchased shares that are moved to sold shares
        private async Task<string> EditSell(SharesPurchaseds model, SharesPurchasedViewModel vm, bool import)
        {
            try
            {
                if (vm is null || model is null)
                    return await HandleError(vm, "Radera sålda", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

                double howManyLeft = model.HowMany - vm.SaleHowMany;
                double residualValue = howManyLeft * Math.Round(model.PricePerShares, 2, MidpointRounding.AwayFromZero);

                model.HowMany -= vm.SaleHowMany;
                model.Amount = residualValue;
                string importTrue = import ? "Ja" : "Nej";

                string note = $"Sålt delar: " +
                              $"\r\nBolag: {vm.CompanyName} " +
                              $"\r\nDatum: {vm.SaleDateOfPurchase:yyyy-MM-dd} " +
                              $"\r\nImport: {importTrue} " +
                              $"\r\nHur många: {vm.SaleHowMany} " +
                              $"\r\nPris per st: {vm.SalePricePerShares} " +
                              $"\r\nSälj värdet: {vm.SaleHowMany * vm.SalePricePerShares} " +
                              $"\r\nKvarvarande värde: {residualValue} " +
                              $"\r\nCourtage: {vm.SaleBrokerage} " +
                              $"\r\nImport: {importTrue} ";

                model.Note = string.IsNullOrEmpty(model.Note) ? note : model.Note + note;

                await db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Radera sålda", import, ex.Message);
            }
        }

        public async Task<string> Delete(SharesPurchaseds incomingModel, SharesPurchasedViewModel vm, bool import)
        {
            try
            {
                if (vm == null || vm.SharesPurchasedId == 0 || _db == null)
                    return await HandleError(vm, "Ta bort", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");

                _db.ChangeTracker.Clear();

                if (import && incomingModel != null && incomingModel.SharesPurchasedId > 0)
                    _db.SharesPurchaseds.Remove(incomingModel);
                else
                {
                    SharesPurchaseds model = ChangesFromViewModelToModel(vm);
                    _db.SharesPurchaseds.Remove(model);
                }

                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Ta bort såld", import, ex.Message);
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesPurchasedViewModel ChangeFromModelToViewModel(SharesPurchaseds model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesPurchasedViewModel vm = _mapper.Map<SharesPurchasedViewModel>(model);

            if (!string.IsNullOrEmpty(model.DateOfPurchase))
                vm.DateOfPurchase = ParseDate(model.DateOfPurchase);

            return vm;
        }

        public SharesPurchasedViewModel ChangeFromImportSellToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);
            string amount = (double.Parse(model.NumberOfSharesString) * double.Parse(model.PricePerShareString)).ToString();

            SharesPurchasedViewModel vm = new()
            {
                SaleDateOfPurchase = date,
                CompanyName = model.CompanyOrInformation,
                SaleHowMany = int.Parse(model.NumberOfSharesString),
                SalePricePerShares = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(amount), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
            };

            if (vm.Account == "Aktieinvest")
            {
                if (model.AmountString.Contains('-'))
                    vm.SaleBrokerage = double.Parse(amount) - double.Parse(model.AmountString.Substring(1));
                else
                    vm.SaleBrokerage = double.Parse(amount) - double.Parse(model.AmountString);
            }
            else
                vm.Brokerage = double.Parse(model.BrokerageString);

            return vm;
        }

        public SharesPurchasedViewModel ChangeFromImportAddToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);
            string amount = (double.Parse(model.NumberOfSharesString) * double.Parse(model.PricePerShareString)).ToString();

            SharesPurchasedViewModel vm = new()
            {
                DateOfPurchase = date,
                CompanyName = model.CompanyOrInformation,
                HowMany = int.Parse(model.NumberOfSharesString),
                PricePerShares = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(amount), 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
            };

            if (vm.Account == "Aktieinvest")
            {
                if (model.AmountString.Contains('-'))
                    vm.Brokerage = double.Parse(model.AmountString.Substring(1)) - double.Parse(amount);
                else
                    vm.Brokerage = double.Parse(model.AmountString) - double.Parse(amount);
            }
            else
                vm.Brokerage = double.Parse(model.BrokerageString);

            return vm;
        }

        private SharesPurchaseds ChangesFromViewModelToModel(SharesPurchasedViewModel vm)
        {
            ArgumentNullException.ThrowIfNull(vm);

            SharesPurchaseds model = _mapper.Map<SharesPurchaseds>(vm);

            if (vm.DateOfPurchase != DateTime.MinValue)
                model.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");

            model.PricePerShares = Math.Round(double.Parse(vm.PricePerShares!), 2, MidpointRounding.AwayFromZero);
            model.Brokerage = Math.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero);
            model.Amount = Math.Round(vm.HowMany * double.Parse(vm.PricePerShares!), 2, MidpointRounding.AwayFromZero);

            return model;
        }

        private static SharesFeeViewModel ChangeFromToPurchasedToFeeViewModel(SharesPurchasedViewModel vm, double brokerage, string note)
        {
            SharesFeeViewModel fee = new()
            {
                Date = DateTime.Now,
                CompanyOrInformation = vm.CompanyName,
                Brokerage = brokerage,
                Note = note,

                // For error information
                DateOfFee = vm.DateOfPurchase,
                Account = vm.Account,
                TypeOfTransaction = "Sälj aktie",
                ISIN = vm.ISIN
            };

            return fee;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private async Task<string> HandleError(SharesPurchasedViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }

        private async Task ErrorHandling(SharesPurchasedViewModel? vm, string type, bool import, string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(vm);

            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    CompanyOrInformation = vm.CompanyName,
                    TypeOfTransaction = type + " aktie",
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} aktie: " +
                       $"\r\nKöp datum: {vm.DateOfPurchase} " +
                       $"\r\nImport: {importTrue} " +
                       $"\r\nId: {vm.SharesPurchasedId} " +
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