
using AutoMapper;
using MyPrivateApp.Components.FarmWork.Classes;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesIndexYearsClass(ApplicationDbContext db, ILogger<FarmWorkClass> logger, IMapper mapper) : ISharesIndexYearsClass
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ILogger<FarmWorkClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<SharesTotalProfitsOrLosses> GetTotalProfitsOrLosses(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return await _db.SharesTotalProfitsOrLosses.FirstOrDefaultAsync(r => r.SharesTotalProfitOrLossId == id)
                   ?? throw new Exception("Totala summan hittades inte i databasen!");
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        public async Task<string> CalculateLastYearsResults()
        {
            if (_db == null)
                return "Ingen kontakt med databasen";

            if (!_db.SharesProfitOrLossYears.Any())
                return "Befintlig databas är tom!";

            int thisYear = 2012; //DateTime.Now.Year; // Magnus: Ändra tillbaka
            double sharesPurchaseds, sharesSolds = 0, fundsPurchased, fundsSold = 0;
            double dividends, interestRates, fees = 0, taxes = 0, brokerage = 0;
            string thisYearsErrorMessage = "Man kan inte beräknad detta året än, man måste vänta tills efter nyår!";
            int thisCalculationYear = 0, biggerYear = 0;

            if (!_db.SharesProfitOrLossYears.Any())
                return "Finns inget rad i tabellen: SharesProfitOrLossYears. Måste finns minst en rad!";

            foreach (var item in _db.SharesProfitOrLossYears)
            {
                if (string.IsNullOrEmpty(item.Year))
                    return "År saknas i tabellen: SharesProfitOrLossYears. Måste fyllas i!";

                int itemYear = int.Parse(item.Year);
                if (itemYear > biggerYear)
                {
                    thisCalculationYear = itemYear + 1;
                    biggerYear = itemYear;
                }
            }

            sharesPurchaseds = CalculateShares(_db.SharesSolds, thisYear, thisCalculationYear, thisYearsErrorMessage, ref sharesSolds);
            fundsPurchased = CalculateFunds(_db.SharesSoldFunds, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fundsSold);
            dividends = CalculateDividends(_db.SharesDividends, thisYear, thisCalculationYear, thisYearsErrorMessage);
            interestRates = CalculateInterestRates(_db.SharesInterestRates, thisYear, thisCalculationYear, thisYearsErrorMessage);
            CalculateFees(_db.SharesFees, thisYear, thisCalculationYear, thisYearsErrorMessage, ref fees, ref taxes, ref brokerage);

            try
            {
                double sharesYearResult = sharesSolds - sharesPurchaseds;
                double fundsYearResult = fundsSold - fundsPurchased;
                double moneyProfitOrLossYear = (sharesYearResult + fundsYearResult + dividends + interestRates) - (fees + taxes + brokerage);
                double soldCalculation = (sharesSolds + fundsSold + dividends + interestRates) - (fees + taxes + brokerage);
                double purchasedSharesAndFunds = sharesPurchaseds + fundsPurchased;
                double percentProfitOrLossYear = (soldCalculation / purchasedSharesAndFunds) - 1;

                SharesProfitOrLossYears model = new()
                {
                    Year = thisCalculationYear.ToString(),
                    SharesYear = double.Round(sharesYearResult, 2, MidpointRounding.AwayFromZero),
                    FundsYear = double.Round(fundsYearResult, 2, MidpointRounding.AwayFromZero),
                    DividendYear = double.Round(dividends, 2, MidpointRounding.AwayFromZero),
                    InterestRatesYear = double.Round(interestRates, 2, MidpointRounding.AwayFromZero),
                    FeeYear = double.Round(fees, 2, MidpointRounding.AwayFromZero),
                    TaxYear = double.Round(taxes, 2, MidpointRounding.AwayFromZero),
                    BrokerageYear = double.Round(brokerage, 2, MidpointRounding.AwayFromZero),
                    MoneyProfitOrLossYear = double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero),
                    PercentProfitOrLossYear = ConvertToPercentage(percentProfitOrLossYear),
                    Note = $"Sålda Aktier: {sharesSolds} - Köpta aktier: {sharesPurchaseds} = {sharesSolds - sharesPurchaseds}" +
                           $"\r\nSålda fonder: {fundsSold} - Köpta fonder: {fundsPurchased} = {fundsSold - fundsPurchased}" +
                           $"\r\nUtdelning: {dividends}" +
                           $"\r\nSkatter: {taxes} + Avgifter: {fees} + Courtage: {brokerage} = {double.Round(taxes + fees + brokerage, 2, MidpointRounding.AwayFromZero)}"
                };

                await _db.SharesProfitOrLossYears.AddAsync(model);
                SharesTotalProfitsOrLosses? sharesTotalProfitsOrLosses = await _db.SharesTotalProfitsOrLosses.FirstOrDefaultAsync();

                if (sharesTotalProfitsOrLosses != null)
                    sharesTotalProfitsOrLosses.TotalProfitOrLoss += double.Round(moneyProfitOrLossYear, 2, MidpointRounding.AwayFromZero);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return $"Lägg till årligt resultat. Felmeddelande: {ex.Message}";
            }

            return string.Empty;
        }

        private static double CalculateShares(IEnumerable<SharesSolds> sharesSolds, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double sharesSoldsTotal)
        {
            double sharesPurchaseds = 0;
            foreach (var item in sharesSolds)
            {
                int checkYear = DateTime.Parse(item.DateOfSold).Year;
                if (checkYear == thisYear) throw new InvalidOperationException("Aktier: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    sharesPurchaseds += item.Amount;
                    sharesSoldsTotal += item.AmountSold;
                    item.CalculationFlag = true;
                }
            }
            return sharesPurchaseds;
        }

        private static double CalculateFunds(IEnumerable<SharesSoldFunds> sharesSoldFunds, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double fundsSoldTotal)
        {
            double fundsPurchased = 0;
            foreach (var item in sharesSoldFunds)
            {
                int checkYear = DateTime.Parse(item.DateOfSold).Year;
                if (checkYear == thisYear) throw new InvalidOperationException("Fonder: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    fundsPurchased += item.Amount;
                    fundsSoldTotal += item.AmountSold;
                    item.CalculationFlag = true;
                }
            }
            return fundsPurchased;
        }

        private static double CalculateDividends(IEnumerable<SharesDividend> sharesDividends, int thisYear, int thisCalculationYear, string thisYearsErrorMessage)
        {
            double dividends = 0;
            foreach (var item in sharesDividends)
            {
                int checkYear = DateTime.Parse(item.Date).Year;
                if (checkYear == thisYear) throw new InvalidOperationException("Utdelaning: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    dividends += item.TotalAmount;
                    item.CalculationFlag = true;
                }
            }
            return dividends;
        }

        private static double CalculateInterestRates(IEnumerable<SharesInterestRates> sharesInterestRates, int thisYear, int thisCalculationYear, string thisYearsErrorMessage)
        {
            double interestRates = 0;
            foreach (var item in sharesInterestRates)
            {
                int checkYear = DateTime.Parse(item.Date).Year;
                if (checkYear == thisYear) throw new InvalidOperationException("Ränta: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    interestRates += item.TotalAmount;
                    item.CalculationFlag = true;
                }
            }
            return interestRates;
        }

        private static void CalculateFees(IEnumerable<SharesFee> sharesFees, int thisYear, int thisCalculationYear, string thisYearsErrorMessage, ref double fees, ref double taxes, ref double brokerage)
        {
            foreach (var item in sharesFees)
            {
                int checkYear = DateTime.Parse(item.Date).Year;
                if (checkYear == thisYear) throw new InvalidOperationException("Kostnader: " + thisYearsErrorMessage);
                if (checkYear == thisCalculationYear && !item.CalculationFlag)
                {
                    fees += item.Fee;
                    taxes += item.Tax;
                    brokerage += item.Brokerage;
                    item.CalculationFlag = true;
                }
            }
        }

        public async Task<string> Delete(SharesProfitOrLossYears model)
        {
            if (model == null || _db == null || model.SharesProfitOrLossYearsId <= 0)
                return "Hittar ingen data från formuläret eller databasen!";

            try
            {
                _db.ChangeTracker.Clear();
                _db.SharesProfitOrLossYears.Remove(model);
                await _db.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Ta bort. Felmeddelande: {ex.Message}";
            }
        }

        public SharesProfitOrLossYearViewModel ChangeFromModelToViewModel(SharesProfitOrLossYears model)
        {
            SharesProfitOrLossYearViewModel vm = new()
            {
                SharesProfitOrLossYearsId = model.SharesProfitOrLossYearsId,
                Year = model.Year,
                SharesYear = model.SharesYear.ToString("#,##0.00"),
                FundsYear = model.FundsYear.ToString("#,##0.00"),
                DividendYear = model.DividendYear.ToString("#,##0.00"),
                InterestRatesYear = model.InterestRatesYear.ToString("#,##0.00"),
                FeeYear = model.FeeYear.ToString("#,##0.00"),
                TaxYear = model.TaxYear.ToString("#,##0.00"),
                BrokerageYear = model.BrokerageYear.ToString("#,##0.00"),
                MoneyProfitOrLossYear = model.MoneyProfitOrLossYear.ToString("#,##0.00"),
                PercentProfitOrLossYear = model.PercentProfitOrLossYear,
                Note = model.Note
            };

            return vm;
        }

        //private static SharesProfitOrLossYears ChangeFromViewModelToModel(SharesProfitOrLossYearViewModel vm)
        //{
        //    SharesProfitOrLossYears model = new()
        //    {
        //        SharesProfitOrLossYearsId = vm.SharesProfitOrLossYearsId,
        //        Year = vm.Year,
        //        SharesYear = double.Parse(vm.SharesYear),
        //        FundsYear = double.Parse(vm.FundsYear),
        //        DividendYear = double.Parse(vm.DividendYear),
        //        InterestRatesYear = double.Parse(vm.InterestRatesYear),
        //        FeeYear = double.Parse(vm.FeeYear),
        //        TaxYear = double.Parse(vm.TaxYear),
        //        BrokerageYear = double.Parse(vm.BrokerageYear),
        //        MoneyProfitOrLossYear = double.Parse(vm.MoneyProfitOrLossYear),
        //        PercentProfitOrLossYear = vm.PercentProfitOrLossYear,
        //        Note = vm.Note
        //    };

        //    return model;
        //}
    }
}