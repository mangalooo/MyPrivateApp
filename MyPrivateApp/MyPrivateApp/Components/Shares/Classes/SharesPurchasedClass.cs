using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;
using System;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedClass : ISharesPurchasedClass
    {
        private static SharesPurchaseds? Get(ApplicationDbContext db, string ISIN) => db.SharesPurchaseds.Any(r => r.ISIN == ISIN) ?
                                                                                            db.SharesPurchaseds.FirstOrDefault(r => r.ISIN == ISIN) :
                                                                                                throw new Exception("Aktien hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) && !string.IsNullOrEmpty(vm.ISIN) &&
                       vm.HowMany > 0 && vm.PricePerShares > 0 && vm.Brokerage > 0)
                {
                    SharesPurchaseds model = ChangesFromViewModelToModel(vm);
                    model.Note += $"Import: {importTrue}, Köper: {model.CompanyName} aktier, Datum: {model.DateOfPurchase.ToString()[..10]}, Hur många: {model.HowMany} " +
                                  $"Pris per st: {model.PricePerShares}, Summan: {model.HowMany * model.PricePerShares}, Courtage: {model.Brokerage}. ";

                    try
                    {
                        db.SharesPurchaseds.Add(model);
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
                        return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!";
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

        public string Edit(ApplicationDbContext db, SharesPurchasedViewModel vm)
        {
            if (vm != null && vm.SharesPurchasedId > 0 && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) &&
                    !string.IsNullOrEmpty(vm.ISIN) && vm.HowMany > 0 && vm.PricePerShares > 0 && vm.Brokerage > 0)
                {
                    try
                    {
                        SharesPurchaseds dbModel = Get(db, vm.ISIN);

                        if (dbModel != null)
                        {
                            dbModel.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
                            dbModel.CompanyName = vm.CompanyName;
                            dbModel.HowMany = vm.HowMany;
                            dbModel.PricePerShares = vm.PricePerShares;
                            dbModel.Brokerage = vm.Brokerage;
                            dbModel.Currency = vm.Currency;
                            dbModel.ISIN = vm.ISIN;
                            dbModel.Account = vm.Account;
                            dbModel.Amount = vm.HowMany * vm.PricePerShares;
                            dbModel.TypeOfShares = vm.TypeOfShares;
                            dbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Aktien hittades inte i databasen!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra köpt", false, ex.Message);
                    }
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!";
            }
            else
                return "Aktien hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public string AddMore(ApplicationDbContext db, SharesPurchasedViewModel moreVM, bool import)
        {
            if (moreVM != null && moreVM.SharesPurchasedId > 0 && !string.IsNullOrEmpty(moreVM.ISIN) && db != null)
            {
                if (import == false)
                    if (moreVM.MoreDateOfPurchase == DateTime.MinValue || moreVM.MoreHowMany == 0 || moreVM.MorePricePerShares == 0 || moreVM.MoreBrokerage == 0)
                        return "Du måste fylla i fälten: Köp mer: Datum, Köp mer: Antal, Köp mer: Pris per aktie, Köp mer: Courage!";

                string importTrue = import ? "Ja" : "Nej";

                SharesPurchaseds DbModel = Get(db, moreVM.ISIN);

                if (DbModel != null)
                {
                    DbModel.HowMany += moreVM.MoreHowMany;
                    DbModel.Brokerage += moreVM.MoreBrokerage;
                    DbModel.Amount += moreVM.MoreHowMany * moreVM.MorePricePerShares;
                    DbModel.PricePerShares = DbModel.Amount / DbModel.HowMany;
                    DbModel.Note += $" |*** Import: {importTrue}, Köper mer aktier för {moreVM.CompanyName}: Datum: " +
                        $"{moreVM.MoreDateOfPurchase.ToString()[..10]}, Hur många: {moreVM.MoreHowMany}, Pris per st: " +
                        $"{moreVM.MorePricePerShares}, Summan: {moreVM.MoreHowMany * moreVM.MorePricePerShares}, " +
                        $"Courtage: {moreVM.MoreBrokerage}. ";

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

        // Selling all or part of the share
        public string Sell(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import, ISharesFeeClass sharesFeeClass)
        {
            SharesPurchaseds sharesPurchaseds = Get(db, vm.ISIN);

            if (sharesPurchaseds == null) return "Sälj: Aktien hittades inte i databasen!";

            vm.SharesPurchasedId = sharesPurchaseds.SharesPurchasedId;

            if (vm != null && vm.SharesPurchasedId != 0 && db != null)
            {
                if (import == false)
                    if (vm.SaleDateOfPurchase == DateTime.MinValue && vm.SaleHowMany > 0 && vm.SalePricePerShares > 0 && vm.SaleBrokerage > 0)
                        return "Du måste fylla i fälten: Sälj: Datum, Sälj: Antal, Sälj: Pris per aktie, Sälj: Courage!";

                string importTrue = import ? "Ja" : "Nej";

                SharesPurchaseds getDbSharesPurchasedsModel = Get(db, vm.ISIN);

                if (getDbSharesPurchasedsModel != null)
                {
                    // Selling the entire share
                    if (getDbSharesPurchasedsModel.HowMany == vm.SaleHowMany)
                    {
                        SharesSolds shares = new()
                        {
                            DateOfPurchase = getDbSharesPurchasedsModel.DateOfPurchase,
                            DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                            Amount = getDbSharesPurchasedsModel.PricePerShares * vm.SaleHowMany,
                            Brokerage = getDbSharesPurchasedsModel.Brokerage + vm.SaleBrokerage,
                            CompanyName = getDbSharesPurchasedsModel.CompanyName,
                            HowMany = vm.SaleHowMany,
                            TypeOfShares = getDbSharesPurchasedsModel.TypeOfShares,
                            Currency = getDbSharesPurchasedsModel.Currency,
                            ISIN = getDbSharesPurchasedsModel.ISIN,
                            Account = getDbSharesPurchasedsModel.Account,
                            PricePerShares = getDbSharesPurchasedsModel.PricePerShares,
                            PricePerSharesSold = vm.SalePricePerShares,
                            AmountSold = vm.SalePricePerShares * vm.SaleHowMany,
                            Note = $"{getDbSharesPurchasedsModel.Note} |*** Import: {importTrue}, Sålt aktien: {getDbSharesPurchasedsModel.CompanyName}, Datum: {vm.SaleDateOfPurchase.ToString()[..10]}, " +
                                   $"Hur många: {vm.SaleHowMany}, Pris per st: {vm.SalePricePerShares}, " +
                                   $"Summan: {vm.SaleHowMany * vm.SalePricePerShares}, Courtage: {getDbSharesPurchasedsModel.Brokerage + vm.SaleBrokerage}. "
                        };

                        shares.MoneyProfitOrLoss = shares.AmountSold - shares.Amount;

                        double calculateMoneyProfitOrLoss = (shares.AmountSold / shares.Amount) - 1;

                        shares.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

                        try
                        {
                            db.SharesSolds.Add(shares);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandling(db, vm, "Såld lägg till", import, ex.Message);
                        }

                        // Brokerage must be added to the fee table!
                        SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(shares.Brokerage, $"Courtage för aktien: {vm.CompanyName}");
                        sharesFeeClass.Add(db, FeeVM, import);

                        // Removes the bought shares that is moved to sold shares
                        Delete(db, getDbSharesPurchasedsModel, vm, import);
                    }
                    else
                    {
                        // Selling parts of the shares
                        SharesSolds shares = new()
                        {
                            DateOfPurchase = getDbSharesPurchasedsModel.DateOfPurchase,
                            DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                            Amount = getDbSharesPurchasedsModel.PricePerShares * vm.SaleHowMany,
                            Brokerage = vm.SaleBrokerage,
                            CompanyName = getDbSharesPurchasedsModel.CompanyName,
                            HowMany = vm.SaleHowMany,
                            TypeOfShares = getDbSharesPurchasedsModel.TypeOfShares,
                            Currency = getDbSharesPurchasedsModel.Currency,
                            ISIN = getDbSharesPurchasedsModel.ISIN,
                            Account = getDbSharesPurchasedsModel.Account,
                            PricePerShares = getDbSharesPurchasedsModel.PricePerShares,
                            PricePerSharesSold = vm.SalePricePerShares,
                            AmountSold = vm.SalePricePerShares * vm.SaleHowMany,
                            Note = $"{getDbSharesPurchasedsModel.Note} |*** Import: {importTrue}, Sålt delar av aktien: {getDbSharesPurchasedsModel.CompanyName}, Datum: {vm.SaleDateOfPurchase.ToString()[..10]}, " +
                                   $"Hur många: {vm.SaleHowMany}, Pris per st: {vm.SalePricePerShares}, " +
                                   $"Summan: {vm.SaleHowMany * vm.SalePricePerShares}, Courtage: {vm.SaleBrokerage}. "
                        };

                        shares.MoneyProfitOrLoss = shares.AmountSold - shares.Amount;

                        double calculateMoneyProfitOrLoss = (shares.AmountSold / shares.Amount) - 1;

                        shares.PercentProfitOrLoss = ConvertToPercentage(calculateMoneyProfitOrLoss);

                        try
                        {
                            db.SharesSolds.Add(shares);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandling(db, vm, "Sålt delar av", import, ex.Message);
                        }

                        // Brokerage must be added to the fee table! (For the parts that were sold)
                        SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(shares.Brokerage, $"Courtage för sålda delar av aktien: {vm.CompanyName}");
                        sharesFeeClass.Add(db, FeeVM, false);

                        // Removes portions of the purchased shares that are moved to sold shares
                        EditSell(db, getDbSharesPurchasedsModel, vm, import);
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

        // Removes portions of the purchased shares that are moved to sold shares
        private static void EditSell(ApplicationDbContext db, SharesPurchaseds dbModel, SharesPurchasedViewModel vm, bool import)
        {
            if (vm != null && dbModel != null && db != null)
            {
                string importTrue = import ? "Ja" : "Nej";

                try
                {
                    dbModel.HowMany -= vm.SaleHowMany;
                    dbModel.Amount = dbModel.HowMany * vm.PricePerShares;
                    dbModel.Note = vm.Note + $"|*** Import: {importTrue},  Sålt delar av aktien  {vm.CompanyName}: " +
                        $"Datum: {vm.SaleDateOfPurchase.ToString()[..10]} Hur många: {vm.SaleHowMany} " +
                        $"Pris per st: {vm.SalePricePerShares} Summan:  {vm.SaleHowMany * vm.SalePricePerShares}, " +
                        $"Courtage: {vm.SaleBrokerage}  ";

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

        public string Delete(ApplicationDbContext db, SharesPurchaseds incomingModel, SharesPurchasedViewModel vm, bool import)
        {
            if (vm != null && vm.SharesPurchasedId != 0  && db != null)
            {
                try
                {
                    db.ChangeTracker.Clear();

                    if (import && incomingModel != null && incomingModel.SharesPurchasedId > 0)
                        db.SharesPurchaseds.Remove(incomingModel);
                    else
                    {
                        SharesPurchaseds model = ChangesFromViewModelToModel(vm);
                        db.SharesPurchaseds.Remove(model);
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

        public SharesPurchasedViewModel ChangeFromModelToViewModel(SharesPurchaseds model)
        {
            DateTime date = DateTime.Parse(model.DateOfPurchase);

            SharesPurchasedViewModel vm = new()
            {
                SharesPurchasedId = model.SharesPurchasedId,
                DateOfPurchase = date,
                CompanyName = model.CompanyName,
                HowMany = model.HowMany,
                PricePerShares = double.Round(model.PricePerShares, 2, MidpointRounding.AwayFromZero),
                Brokerage = model.Brokerage,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero),
                TypeOfShares = model.TypeOfShares,
                Note = model.Note
            };

            return vm;
        }

        public SharesPurchasedViewModel ChangeFromImportSellToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesPurchasedViewModel vm = new()
            {
                SaleDateOfPurchase = date,
                CompanyName = model.CompanyOrInformation,
                SaleHowMany = int.Parse(model.NumberOfSharesString),
                SalePricePerShares = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                SaleBrokerage = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero),
            };

            return vm;
        }

        public SharesPurchasedViewModel ChangeFromImportAddToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesPurchasedViewModel vm = new()
            {
                DateOfPurchase = date,
                CompanyName = model.CompanyOrInformation,
                HowMany = int.Parse(model.NumberOfSharesString),
                PricePerShares = double.Round(double.Parse(model.PricePerShareString), 2, MidpointRounding.AwayFromZero),
                Brokerage = double.Parse(model.BrokerageString),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.AccountNumber,
                Amount = double.Round(double.Parse(model.AmountString), 2, MidpointRounding.AwayFromZero),
            };

            return vm;
        }

        private static SharesPurchaseds ChangesFromViewModelToModel(SharesPurchasedViewModel vm)
        {
            SharesPurchaseds sharesPurchased = new()
            {
                SharesPurchasedId = vm.SharesPurchasedId,
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = double.Round(vm.PricePerShares, 2, MidpointRounding.AwayFromZero),
                Brokerage = vm.Brokerage,
                Amount = double.Round(vm.HowMany * vm.PricePerShares, 2, MidpointRounding.AwayFromZero),
                ISIN = vm.ISIN,
                Currency = vm.Currency,
                Account = vm.Account,
                TypeOfShares = vm.TypeOfShares,
                Note = vm.Note
            };

            return sharesPurchased;
        }

        private static SharesFeeViewModel ChangeFromToPurchasedToFeeViewModel(double brokerage, string note)
        {
            SharesFeeViewModel fee = new()
            {
                Date = DateTime.Now,
                Brokerage = brokerage,
                Note = note
            };

            return fee;
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";

        private static void ErrorHandling(ApplicationDbContext db, SharesPurchasedViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"Import: {importTrue}, {type} aktie: {DateTime.Now}: Företag: {vm.CompanyName}, " +
                        $"Datum: {vm.DateOfPurchase}, Id: {vm.SharesPurchasedId}, ISIN: {vm.ISIN}."
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}