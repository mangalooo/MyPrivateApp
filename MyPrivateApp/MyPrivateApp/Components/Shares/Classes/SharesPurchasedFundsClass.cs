﻿using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedFundsClass : ISharesPurchasedFundsClass
    {
        private static SharesPurchasedFunds? Get(ApplicationDbContext db, string ISIN) => db.SharesPurchasedFunds.Any(r => r.ISIN == ISIN) ?
                                                                                            db.SharesPurchasedFunds.FirstOrDefault(r => r.ISIN == ISIN) :
                                                                                                throw new Exception("Fonden hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesPurchasedFundViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.FundName) && !string.IsNullOrEmpty(vm.ISIN) &&
                       vm.HowMany > 0 && vm.PricePerFunds > 0 && vm.Fee > 0)
                {
                    SharesPurchasedFunds model = ChangesFromViewModelToModel(vm);
                    model.Note += $"Import: {importTrue}, Köper: {model.FundName} fond, Datum: {model.DateOfPurchase.ToString()[..10]}, Hur många: {model.HowMany} " +
                                  $"Pris per st: {model.PricePerFunds}, Summan: {model.HowMany * model.PricePerFunds}, Avgift: {model.Fee}. ";

                    try
                    {
                        db.SharesPurchasedFunds.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Köpt", import, ex.Message);
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Köpt", import, "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!");
                    else
                        return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Köpt", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            }

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, SharesPurchasedFundViewModel vm)
        {
            if (vm != null && vm.SharesPurchasedFundId > 0 && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.FundName) &&
                    !string.IsNullOrEmpty(vm.ISIN) && vm.HowMany > 0 && vm.PricePerFunds > 0 && vm.Fee > 0)
                {
                    try
                    {
                        SharesPurchasedFunds dbModel = Get(db, vm.ISIN);

                        if (dbModel != null)
                        {
                            dbModel.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
                            dbModel.FundName = vm.FundName;
                            dbModel.HowMany = vm.HowMany;
                            dbModel.PricePerFunds = vm.PricePerFunds;
                            dbModel.Fee = vm.Fee;
                            dbModel.Currency = vm.Currency;
                            dbModel.ISIN = vm.ISIN;
                            dbModel.Account = vm.Account;
                            dbModel.Amount = vm.HowMany * vm.PricePerFunds;
                            dbModel.TypeOfFund = vm.TypeOfFund;
                            dbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Fondem hittades inte i databasen!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra köpt", false, ex.Message);
                    }
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per fond del och Avgift!";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string AddMore(ApplicationDbContext db, SharesPurchasedFundViewModel moreVM, bool import)
        {
            if (moreVM != null && moreVM.SharesPurchasedFundId > 0 && !string.IsNullOrEmpty(moreVM.ISIN) && db != null)
            {
                if (import == false)
                    if (moreVM.MoreDateOfPurchase == DateTime.MinValue || moreVM.MoreHowMany == 0 || moreVM.MorePricePerFunds == 0 || moreVM.MoreFee == 0)
                        return "Du måste fylla i fälten: Köp mer: Datum, Köp mer: Antal, Köp mer: Pris per fond el, Köp mer: Avgift!";

                string importTrue = import ? "Ja" : "Nej";

                SharesPurchasedFunds DbModel = Get(db, moreVM.ISIN);

                if (DbModel != null)
                {
                    DbModel.HowMany += moreVM.MoreHowMany;
                    DbModel.Fee += moreVM.MoreFee;
                    DbModel.Amount += moreVM.MoreHowMany * moreVM.MorePricePerFunds;
                    DbModel.PricePerFunds = DbModel.Amount / DbModel.HowMany;
                    DbModel.Note += $" |*** Import: {importTrue}, Köper mer fond delar för {moreVM.FundName}: Datum: " +
                        $"{moreVM.MoreDateOfPurchase.ToString()[..10]}, Hur många: {moreVM.MoreHowMany}, Pris per st: " +
                        $"{moreVM.MorePricePerFunds}, Summan: {moreVM.MoreHowMany * moreVM.MorePricePerFunds}, " +
                        $"Courtage: {moreVM.MoreFee}. ";

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, moreVM, "Köpt mera", import, ex.Message);
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, moreVM, "Köpt mera", import, "Hittar inte aktien i databasen!");
                    else
                        return "Hittar inte aktien i databasen!";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, moreVM, "Köpt mera", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            }

            return string.Empty;
        }

        // Selling all or part of the fund
        public string Sell(ApplicationDbContext db, SharesPurchasedFundViewModel vm, bool import, ISharesFeeClass sharesFeeClass)
        {
            SharesPurchasedFunds fundsPurchaseds = Get(db, vm.ISIN);

            if (fundsPurchaseds == null) return "Sälj: Aktien hittades inte i databasen!";

            vm.SharesPurchasedFundId = fundsPurchaseds.SharesPurchasedFundId;

            if (vm != null && vm.SharesPurchasedFundId != 0 && db != null)
            {
                if (import == false)
                    if (vm.SaleDateOfPurchase == DateTime.MinValue && vm.SaleHowMany > 0 && vm.SalePricePerFunds > 0 && vm.SaleFee > 0)
                        return "Du måste fylla i fälten: Sälj: Datum, Sälj: Antal, Sälj: Pris per fond del, Sälj: Avgift!";

                string importTrue = import ? "Ja" : "Nej";

                SharesPurchasedFunds getDbFundsPurchasedsModel = Get(db, vm.ISIN);

                if (getDbFundsPurchasedsModel != null)
                {
                    // Selling the entire share
                    if (getDbFundsPurchasedsModel.HowMany == vm.SaleHowMany)
                    {
                        SharesSoldFunds fund = new()
                        {
                            DateOfPurchase = getDbFundsPurchasedsModel.DateOfPurchase,
                            DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                            Amount = getDbFundsPurchasedsModel.PricePerFunds * vm.SaleHowMany,
                            Fee = getDbFundsPurchasedsModel.Fee + vm.SaleFee,
                            FundName = getDbFundsPurchasedsModel.FundName,
                            HowMany = vm.SaleHowMany,
                            TypeOfFund = getDbFundsPurchasedsModel.TypeOfFund,
                            Currency = getDbFundsPurchasedsModel.Currency,
                            ISIN = getDbFundsPurchasedsModel.ISIN,
                            Account = getDbFundsPurchasedsModel.Account,
                            PricePerFunds= getDbFundsPurchasedsModel.PricePerFunds,
                            PricePerFundsSold = vm.SalePricePerFunds,
                            AmountSold = vm.SalePricePerFunds * vm.SaleHowMany,
                            Note = $"{getDbFundsPurchasedsModel.Note} |*** Import: {importTrue}, Sålt aktien: {getDbFundsPurchasedsModel.FundName}, " +
                                   $"Datum: {vm.SaleDateOfPurchase.ToString()[..10]}, Hur många: {vm.SaleHowMany}, Pris per st: {vm.SalePricePerFunds}, " +
                                   $"Summan: {vm.SaleHowMany * vm.SalePricePerFunds}, Avgift: {getDbFundsPurchasedsModel.Fee + vm.SaleFee}. "
                        };

                        fund.MoneyProfitOrLoss = fund.AmountSold - fund.Amount;

                        double calculateMoneyProfitOrLoss = (fund.AmountSold / fund.Amount) - 1;

                        fund.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

                        try
                        {
                            db.SharesSoldFunds.Add(fund);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandling(db, vm, "Såld lägg till", import, ex.Message);
                        }

                        // Tax (fee) must be added to the fee table!
                        SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(fund.Fee, $"Avgiften för aktien: {vm.FundName}");
                        sharesFeeClass.Add(db, FeeVM, import);

                        // Removes the bought fund that is moved to sold fund
                        Delete(db, getDbFundsPurchasedsModel, vm, import);
                    }
                    else
                    {
                        // Selling parts of the shares
                        SharesSoldFunds fund = new()
                        {
                            DateOfPurchase = getDbFundsPurchasedsModel.DateOfPurchase,
                            DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                            Amount = getDbFundsPurchasedsModel.PricePerFunds * vm.SaleHowMany,
                            Fee = vm.SaleFee,
                            FundName = getDbFundsPurchasedsModel.FundName,
                            HowMany = vm.SaleHowMany,
                            TypeOfFund = getDbFundsPurchasedsModel.TypeOfFund,
                            Currency = getDbFundsPurchasedsModel.Currency,
                            ISIN = getDbFundsPurchasedsModel.ISIN,
                            Account = getDbFundsPurchasedsModel.Account,
                            PricePerFunds= getDbFundsPurchasedsModel.PricePerFunds,
                            PricePerFundsSold = vm.SalePricePerFunds,
                            AmountSold = vm.SalePricePerFunds * vm.SaleHowMany,
                            Note = $"{getDbFundsPurchasedsModel.Note} |*** Import: {importTrue}, Sålt delar av aktien: {getDbFundsPurchasedsModel.FundName}, Datum: {vm.SaleDateOfPurchase.ToString()[..10]}, " +
                                   $"Hur många: {vm.SaleHowMany}, Pris per st: {vm.SalePricePerFunds}, " +
                                   $"Summan: {vm.SaleHowMany * vm.SalePricePerFunds}, Courtage: {vm.SaleFee}. "
                        };

                        fund.MoneyProfitOrLoss = fund.AmountSold - fund.Amount;

                        double calculateMoneyProfitOrLoss = (fund.AmountSold / fund.Amount) - 1;

                        fund.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

                        try
                        {
                            db.SharesSoldFunds.Add(fund);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandling(db, vm, "Sålt delar av", import, ex.Message);
                        }

                        // Tax (fee) must be added to the fee table! (For the parts that were sold)
                        SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(fund.Fee, $"Courtage för sålda delar av aktien: {vm.FundName}");
                        sharesFeeClass.Add(db, FeeVM, false);

                        // Removes portions of the purchased funds that are moved to sold funds
                        EditSell(db, getDbFundsPurchasedsModel, vm, import);
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Sälj", import, "Hittar inte aktien i databasen!");
                    else
                        return "Hittar inte aktien i databasen!";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Sälj", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            }

            return string.Empty;
        }

        // Removes portions of the purchased funds that are moved to sold funds
        private static void EditSell(ApplicationDbContext db, SharesPurchasedFunds dbModel, SharesPurchasedFundViewModel vm, bool import)
        {
            if (vm != null && dbModel != null && db != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                try
                {
                    dbModel.HowMany -= vm.SaleHowMany;
                    dbModel.Amount = dbModel.HowMany * vm.PricePerFunds;
                    dbModel.Note = vm.Note + $"|*** Import: {importTrue},  Sålt delar av aktien  {vm.FundName}: " +
                        $"Datum: {vm.SaleDateOfPurchase.ToString()[..10]} Hur många: {vm.SaleHowMany} " +
                        $"Pris per st: {vm.SalePricePerFunds} Summan:  {vm.SaleHowMany * vm.SalePricePerFunds}, " +
                        $"Courtage: {vm.SaleFee}  ";

                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Radera sålda", import, ex.Message);
                }
            }
            else
                ErrorHandling(db, vm, "Radera sålda", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
        }

        public string Delete(ApplicationDbContext db, SharesPurchasedFunds model, SharesPurchasedFundViewModel vm, bool import)
        {
            if (vm != null && vm.SharesPurchasedFundId != 0  && db != null)
            {
                try
                {
                    db.ChangeTracker.Clear();

                    if (import && model != null && model.SharesPurchasedFundId > 0)
                        db.SharesPurchasedFunds.Remove(model);
                    else
                    {
                        SharesPurchasedFunds modelDB = ChangesFromViewModelToModel(vm);
                        db.SharesPurchasedFunds.Remove(modelDB);
                    }
                        
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Ta bort såld", import, ex.Message);
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Ta bort såld", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
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
                PricePerFunds = double.Round(model.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                Fee = model.Fee,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero),
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
                SaleHowMany = int.Parse(model.NumberOfSharesString),
                SalePricePerFunds = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                SaleFee = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero),
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
                HowMany = int.Parse(model.NumberOfSharesString),
                PricePerFunds = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                Fee = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero),
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
                PricePerFunds = double.Round(vm.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                Fee = vm.Fee,
                Amount = double.Round(vm.HowMany * vm.PricePerFunds, 2, MidpointRounding.AwayFromZero),
                ISIN = vm.ISIN,
                Currency = vm.Currency,
                Account = vm.Account,
                TypeOfFund = vm.TypeOfFund,
                Note = vm.Note
            };

            return sharesPurchasedFunds;
        }

        private static SharesFeeViewModel ChangeFromToPurchasedToFeeViewModel(double tax, string note)
        {
            SharesFeeViewModel fee = new()
            {
                Date = DateTime.Now,
                Tax = tax,
                Note = note
            };

            return fee;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private static void ErrorHandling(ApplicationDbContext db, SharesPurchasedFundViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} FOND: Fond namn: {vm.FundName}, " +
                        $"Datum: {vm.DateOfPurchase}, Id: {vm.SharesPurchasedFundId}, ISIN: {vm.ISIN}."
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}