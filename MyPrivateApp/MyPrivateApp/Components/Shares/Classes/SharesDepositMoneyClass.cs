
using MyPrivateApp.Components.Enum;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using Microsoft.EntityFrameworkCore;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesDepositMoneyClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesDepositMoneyClass> logger) : ISharesDepositMoneyClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesDepositMoneyClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<SharesTotalAmounts?> GetTotalAmount(int? id)
        {
            if (id <= 0)
                throw new Exception("GetTotalAmount: Finns inget ID!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("GetTotalAmount: db == null!");

            return await db.SharesTotalAmounts.FirstOrDefaultAsync(r => r.TotalAmountId == id)
                ?? throw new Exception("Totala summan hittades inte i databasen!");
        }

        public async Task<string> Add(SharesDepositMoneyViewModel vm, bool import)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Add: db == null!");

                if (vm == null)
                    return await HandleError(vm, "Lägg till", import, "Hittar ingen data från formuläret!");

                SharesTotalAmounts? getTotalAmount = await GetTotalAmount(2); // Should always be just one total amount in the database

                if (getTotalAmount == null)
                    return await HandleError(vm, "Lägg till", import, "Hittar inte de totala beloppet!");

                if (string.IsNullOrEmpty(vm.DepositMoney) || string.IsNullOrEmpty(vm.TypeOfTransaction))
                    return await HandleError(vm, "Lägg till", import, "Inget insatt belopp eller typ av överföring!");

                double amount = ParseAmount(vm.DepositMoney);

                SharesDepositMoney model = ChangeFromViewModelToModel(vm, amount);
                UpdateTotalAmount(getTotalAmount, vm.TypeOfTransaction, amount);

                await db.SharesDepositMoney.AddAsync(model);
                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Lägg till", import, ex.Message);
            }
        }

        private static double ParseAmount(string amountString) => amountString.Contains('-') ? double.Parse(amountString[1..]) : double.Parse(amountString);

        private static void UpdateTotalAmount(SharesTotalAmounts totalAmount, string transactionType, double amount)
        {
            if (transactionType == "Insättning")
                totalAmount.TotalAmount += amount;
            else if (transactionType == "Uttag")
                totalAmount.TotalAmount -= amount;
        }

        public async Task<string> Edit(SharesDepositMoneyViewModel vm)
        {
            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Edit: db == null!");

                if (vm == null || vm.DepositMoneyId <= 0)
                    return "Hittar ingen data från formuläret!";

                SharesDepositMoney? model = await db.SharesDepositMoney.FirstOrDefaultAsync(r => r.DepositMoneyId == vm.DepositMoneyId);
                SharesTotalAmounts? getTotalAmount = await GetTotalAmount(2); // Should always be just one total amount in the database

                if (model == null || getTotalAmount == null || string.IsNullOrEmpty(vm.DepositMoney))
                    return "Hittar inte data från: Kontot, Total summa eller inget insatt belopp!";

                UpdateTotalAmount(getTotalAmount, model.DepositMoney, false); // Update the total amount with the old amount
                UpdateTotalAmount(getTotalAmount, double.Parse(vm.DepositMoney ?? "0"), true); // Update the total amount with the new amount

                EditModel(model, vm);

                await db.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return await HandleError(vm, "Ändra", false, ex.Message);
            }
        }

        private static void UpdateTotalAmount(SharesTotalAmounts totalAmount, double amount, bool isAdding)
        {
            if (isAdding)
                totalAmount.TotalAmount += amount;
            else
                totalAmount.TotalAmount -= amount;
        }

        public async Task<string> Delete(SharesDepositMoney model)
        {
            if (model == null)
                return "Hittar ingen data från formuläret!";

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Delete: db == null!");

                db.ChangeTracker.Clear();
                db.Remove(model);
                await db.SaveChangesAsync();

                SharesTotalAmounts? getTotalAmount = await GetTotalAmount(2); // Should always be just one total amount in the database

                if (getTotalAmount == null)
                    return "Totala summan hittades inte i databasen!";

                getTotalAmount.TotalAmount -= model.DepositMoney;

                await db.SaveChangesAsync();
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Gick inte att ta bort överföringen! Felmeddelande: {ex.Message} ";
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
                return parsedDate;

            return DateTime.MinValue;

            throw new FormatException($"Ogiltigt datumformat: {date}");
        }

        public SharesDepositMoneyViewModel ChangeFromModelToViewModel(SharesDepositMoney model)
        {
            SharesDepositMoneyViewModel vm = new()
            {
                DepositMoneyId = model.DepositMoneyId,
                Date = model.Date != null ? ParseDate(model.Date) : DateTime.MinValue,
                DepositMoney = model.DepositMoney.ToString("#,##0.00"),
                SubmitOrWithdraw = model.SubmitOrWithdraw,
                TypeOfTransaction = model.TypeOfTransaction,
                TransferOptions = model.TransferOptions,
                Account = model.Account,
                Currency = model.Currency,
                Note = model.Note
            };

            return vm;
        }

        private static SharesDepositMoney ChangeFromViewModelToModel(SharesDepositMoneyViewModel vm, double amount)
        {
            SharesDepositMoney model = new()
            {
                DepositMoneyId = vm.DepositMoneyId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                TypeOfTransaction = vm.TypeOfTransaction,
                TransferOptions = vm.TransferOptions,
                Account = vm.Account,
                Currency = vm.Currency,
                Note = vm.Note
            };

            if (string.IsNullOrEmpty(vm.TypeOfTransaction))
            {
                model.DepositMoney = vm.TypeOfTransaction == "Insättning" ? amount : -amount;
                model.SubmitOrWithdraw = vm.TypeOfTransaction == "Insättning" ? SubmitOrWithdraw.Inbetalning : SubmitOrWithdraw.Utbetalning;
            }

            return model;
        }

        public SharesDepositMoneyViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            SharesDepositMoneyViewModel vm = new()
            {
                Date = model.Date != null ? ParseDate(model.Date) : DateTime.MinValue,
                DepositMoney = double.Parse(model.AmountString).ToString("#,##0.00"),
                SubmitOrWithdraw = model.TypeOfTransaction == "Insättning" ? SubmitOrWithdraw.Inbetalning : SubmitOrWithdraw.Utbetalning,
                TransferOptions = model.CompanyOrInformation,
                TypeOfTransaction = model.TypeOfTransaction,
                Account = model.AccountNumber,
                Currency = model.Currency,
            };

            return vm;
        }

        private static void EditModel(SharesDepositMoney model, SharesDepositMoneyViewModel vm)
        {
            model.Date = vm.Date.ToString("yyyy-MM-dd");
            model.DepositMoney = double.Parse(vm.DepositMoney ?? "0");
            model.SubmitOrWithdraw = vm.TypeOfTransaction == "Insättning" ? SubmitOrWithdraw.Inbetalning : SubmitOrWithdraw.Utbetalning;
            model.TypeOfTransaction = vm.TypeOfTransaction;
            model.TransferOptions = vm.TransferOptions;
            model.Account = vm.Account;
            model.Currency = vm.Currency;
            model.Note = vm.Note;
        }

        private async Task ErrorHandling(SharesDepositMoneyViewModel? vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";
            SharesErrorHandlings sharesErrorHandling;

            if (vm == null)
            {
                sharesErrorHandling = new()
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TypeOfTransaction = type,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                };
            }
            else
            {
                sharesErrorHandling = new()
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TypeOfTransaction = type,
                    ErrorMessage = $"Felmeddelande: {errorMessage}",
                    Note = $"{type} BANKÖVERFÖRING: \r\nDatum: {vm.Date:yyyy-MM-dd} \r\nImport: {importTrue} \r\nBelopp: {vm.DepositMoney} \r\nId: {vm.DepositMoneyId}. "
                };
            }

            try
            {
                using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("ErrorHandling: db == null!");

                await db.SharesErrorHandlings.AddAsync(sharesErrorHandling);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel uppstod när felhanteringsinformation skulle sparas!");
            }
        }

        private async Task<string> HandleError(SharesDepositMoneyViewModel? vm, string type, bool import, string errorMessage)
        {
            if (import)
                await ErrorHandling(vm, type, import, errorMessage);

            return $"{type}: Felmeddelande: {errorMessage}";
        }
    }
}