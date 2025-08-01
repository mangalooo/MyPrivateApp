﻿
using MyPrivateApp.Components.Enum;
using MyPrivateApp.Components.Shares.Classes.Interface;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesDepositMoneyClass(IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<SharesDepositMoneyClass> logger, IMapper mapper) : ISharesDepositMoneyClass
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        private readonly ILogger<SharesDepositMoneyClass> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        public async Task<SharesDepositMoney?> Get(int? id)
        {
            if (id <= 0)
                throw new Exception("Get: Finns inget ID!");

            using ApplicationDbContext db = _dbFactory.CreateDbContext() ?? throw new Exception("Get: db == null!");

            return await db.SharesDepositMoney.FirstOrDefaultAsync(r => r.DepositMoneyId == id)
                ?? throw new Exception("Den insatta eller uttagna summan hittades inte i databasen!");
        }

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

                SharesDepositMoney? model = await Get(vm.DepositMoneyId);
                SharesTotalAmounts? getTotalAmount = await GetTotalAmount(2); // Should always be just one total amount in the database

                if (model == null || getTotalAmount == null || string.IsNullOrEmpty(vm.DepositMoney))
                    return "Hittar inte data från: Kontot, Total summa eller inget insatt belopp!";

                UpdateTotalAmount(getTotalAmount, model.DepositMoney, false); // Update the total amount with the old amount
                UpdateTotalAmount(getTotalAmount, double.Parse(vm.DepositMoney), true); // Update the total amount with the new amount

                _mapper.Map(vm, model);

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
            ArgumentNullException.ThrowIfNull(model);

            SharesDepositMoneyViewModel vm = _mapper.Map<SharesDepositMoneyViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            if (model.DepositMoney <= 0)
                vm.DepositMoney = model.DepositMoney.ToString("#,##0.00");

            return vm;
        }

        private SharesDepositMoney ChangeFromViewModelToModel(SharesDepositMoneyViewModel vm, double amount)
        {
            SharesDepositMoney model = _mapper.Map<SharesDepositMoney>(vm);

            if (vm.Date != DateTime.MinValue)
                model.Date = vm.Date.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(vm.TypeOfTransaction))
            {
                model.DepositMoney = vm.TypeOfTransaction == "Insättning" ? amount : -amount;
                model.SubmitOrWithdraw = vm.TypeOfTransaction == "Insättning" ? SubmitOrWithdraw.Inbetalning : SubmitOrWithdraw.Utbetalning;
            }

            return model;
        }

        public SharesDepositMoneyViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            ArgumentNullException.ThrowIfNull(model);

            SharesDepositMoneyViewModel vm = _mapper.Map<SharesDepositMoneyViewModel>(model);

            if (!string.IsNullOrEmpty(model.Date))
                vm.Date = ParseDate(model.Date);

            if (!string.IsNullOrEmpty(model.AmountString))
                vm.DepositMoney = double.Parse(model.AmountString).ToString("#,##0.00");

            return vm;
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