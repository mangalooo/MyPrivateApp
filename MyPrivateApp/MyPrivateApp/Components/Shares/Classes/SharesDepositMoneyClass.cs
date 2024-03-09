using MyPrivateApp.Components.Enum;
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesDepositMoneyClass : ISharesDepositMoneyClass
    {
        private static SharesDepositMoney Get(int? id, ApplicationDbContext db) => db.SharesDepositMoney.FirstOrDefault(r => r.DepositMoneyId == id);

        public SharesTotalAmounts GetTotalAmount(ApplicationDbContext db, int? id) => db.SharesTotalAmounts.FirstOrDefault(r => r.TotalAmountId == id);

        public void Add(ApplicationDbContext db, SharesDepositMoneyViewModel vm, bool import)
        {
            if (vm != null)
            {
                string importTrue = import ? "Ja" : "Nej";
                DateTime ErrorDate = DateTime.Now;
                SharesErrorHandlings sharesErrorHandling;
                SharesDepositMoney model;
                SharesTotalAmounts getTotalAmount = GetTotalAmount(db, 1); // Should always be just one total amount in the database
                string amountString = vm.DepositMoney.ToString();

                double amount = amountString.Contains('-') ? double.Parse(amountString[1..]) : double.Parse(amountString);

                switch (vm.TypeOfTransaction)
                {
                    case "Insättning":

                        model = new()
                        {
                            Date = vm.Date.ToString("yyyy-MM-dd"),
                            DepositMoney = vm.DepositMoney,
                            SubmitOrWithdraw = SubmitOrWithdraw.Inbetalning,
                            TypeOfTransaction = vm.TypeOfTransaction,
                            TransferOptions = vm.TransferOptions,
                            Account = vm.Account,
                            Currency = vm.Currency,
                            Note = vm.Note
                        };

                        getTotalAmount.TotalAmount += amount;

                        try
                        {
                            db.SharesDepositMoney.Add(model);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            sharesErrorHandling = new()
                            {
                                Date = $"{ErrorDate.Year}-{ErrorDate.Month}-{ErrorDate.Day}",
                                ErrorMessage =  $"Felmeddelande: {ex.Message}",
                                Note = $"Import: {importTrue}, Insättning: Date: {vm.Date}, Belopp {vm.DepositMoney}."
                            };

                            db.SharesErrorHandlings.Add(sharesErrorHandling);
                            db.SaveChanges();
                        }

                        break;

                    case "Uttag":

                        model = new()
                        {
                            Date = vm.Date.ToString("yyyy-MM-dd"),
                            DepositMoney = -amount, // Kolla minus tecken
                            SubmitOrWithdraw = SubmitOrWithdraw.Utbetalning,
                            TypeOfTransaction = vm.TypeOfTransaction,
                            TransferOptions = vm.TransferOptions,
                            Account = vm.Account,
                            Currency = vm.Currency,
                            Note = vm.Note
                        };

                        getTotalAmount.TotalAmount -= amount;

                        try
                        {
                            db.SharesDepositMoney.Add(model);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            sharesErrorHandling = new()
                            {
                                Date = $"{ErrorDate.Year}-{ErrorDate.Month}-{ErrorDate.Day}",
                                ErrorMessage = $"Felmeddelande: {ex.Message}",
                                Note = $"Import: {importTrue}, Uttag: Date: {vm.Date}, Belopp {vm.DepositMoney}."
                            };

                            db.SharesErrorHandlings.Add(sharesErrorHandling);
                            db.SaveChanges();
                        }

                        break;

                    default:

                        sharesErrorHandling = new()
                        {
                            Date = $"{ErrorDate.Year}-{ErrorDate.Month}-{ErrorDate.Day}",
                            ErrorMessage = $"Felmeddelande: Inget uttag eller insättning!",
                            Note = $"Import: {importTrue}, Uttag: Date: {vm.Date}, Belopp {vm.DepositMoney}, Transaktion: {vm.TypeOfTransaction}."
                        };

                        db.SharesErrorHandlings.Add(sharesErrorHandling);
                        db.SaveChanges();

                        break;
                }
            }
        }

        public void Edit(ApplicationDbContext db, SharesDepositMoneyViewModel vm)
        {
            if (vm.DepositMoneyId > 0)
            {
                // Remove and add the new amount to the total.
                SharesDepositMoney dbModel = Get(vm.DepositMoneyId, db);
                SharesTotalAmounts getTotalAmount = GetTotalAmount(db, 1); // Should always be just one total amount in the database
                string DBDepositMoneyString = dbModel.DepositMoney.ToString();
                double DBDepositMoney = DBDepositMoneyString.Contains('-') ? double.Parse(DBDepositMoneyString[1..]) : double.Parse(DBDepositMoneyString);
                string VMDepositMoneyString = vm.DepositMoney.ToString();
                double VMDepositMoney = VMDepositMoneyString.Contains('-') ? double.Parse(VMDepositMoneyString[1..]) : double.Parse(VMDepositMoneyString);
                getTotalAmount.TotalAmount -= DBDepositMoney;
                getTotalAmount.TotalAmount += VMDepositMoney;

                // Edit fields
                dbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                dbModel.DepositMoney = vm.DepositMoney;
                dbModel.SubmitOrWithdraw = SubmitOrWithdraw.Utbetalning;
                dbModel.TypeOfTransaction = vm.TypeOfTransaction;
                dbModel.TransferOptions = vm.TransferOptions;
                dbModel.Account = vm.Account;
                dbModel.Currency = vm.Currency;
                dbModel.Note = vm.Note;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    DateTime ErrorDate = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{ErrorDate.Year}-{ErrorDate.Month}-{ErrorDate.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Ändrar {vm.TypeOfTransaction}, Date: {vm.Date}, Belopp {vm.DepositMoney}."
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                return;
            }
        }

        public void Delete(ApplicationDbContext db, SharesDepositMoneyViewModel vm) 
        {
            SharesDepositMoney model = ChangeFromViewModelToModel(vm);

            if (model != null)
            {
                try
                {
                    db.ChangeTracker.Clear();
                    db.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: Ta bort {model.TypeOfTransaction}: Date: {model.Date}, Belopp {model.DepositMoney}."
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                SharesTotalAmounts getTotalAmount = GetTotalAmount(db, 1); // Ska alltid vara bara ett totalt belopp i databasen
                getTotalAmount.TotalAmount -= model.DepositMoney;

                db.SaveChanges();
            }
        }

        public SharesDepositMoneyViewModel ChangeFromModelToViewModel(SharesDepositMoney model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesDepositMoneyViewModel vm = new()
            {
                DepositMoneyId = model.DepositMoneyId,
                Date = date,
                DepositMoney = model.DepositMoney,
                SubmitOrWithdraw = model.SubmitOrWithdraw,
                TypeOfTransaction = model.TypeOfTransaction,
                TransferOptions = model.TransferOptions,
                Account = model.Account,
                Currency = model.Currency,
                Note = model.Note
            };

            return vm;
        }

        public SharesDepositMoneyViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesDepositMoneyViewModel vm = new()
            {
                Account = model.AccountNumber,
                Currency = model.Currency,
                Date = date,
                DepositMoney = double.Parse(model.AmountString),
                TypeOfTransaction = model.CompanyOrInformation,
            };

            return vm;
        }

        private static SharesDepositMoney ChangeFromViewModelToModel(SharesDepositMoneyViewModel vm)
        {
            SharesDepositMoney model = new()
            {
                DepositMoneyId = vm.DepositMoneyId,
                Date = vm.Date.ToString("yyyy-MM-dd"),
                DepositMoney = vm.DepositMoney,
                SubmitOrWithdraw = vm.SubmitOrWithdraw,
                TypeOfTransaction = vm.TypeOfTransaction,
                TransferOptions = vm.TransferOptions,
                Account = vm.Account,
                Currency = vm.Currency,
                Note = vm.Note
            };

            return model;
        }
    }
}