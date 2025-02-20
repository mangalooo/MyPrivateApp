
using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedClass : ISharesPurchasedClass
    {
        private static SharesPurchaseds? Get(ApplicationDbContext db, string ISIN) => db.SharesPurchaseds.Any(r => r.ISIN == ISIN)
                                                                                        ? db.SharesPurchaseds.FirstOrDefault(r => r.ISIN == ISIN)
                                                                                            : throw new Exception("Aktien hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm != null && db != null)
            {
                if (vm.DateOfPurchase != DateTime.MinValue && !string.IsNullOrEmpty(vm.CompanyName) && !string.IsNullOrEmpty(vm.ISIN) &&
                       vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage > 0)
                {
                    SharesPurchaseds model = ChangesFromViewModelToModel(vm);

                    model.Note += $"Köper:" +
                                  $"\r\nBolag: {model.CompanyName} aktier" +
                                  $"\r\nISIN: {vm.ISIN} " +
                                  $"\r\nDatum: {model.DateOfPurchase.ToString()[..10]}" +
                                  $"\r\nHur många: {model.HowMany} " +
                                  $"\r\nPris per st: {model.PricePerShares}" +
                                  $"\r\nVärdet: {model.Amount}" +
                                  $"\r\nCourtage: {model.Brokerage}. " +
                                  $"\r\nImport: {importTrue} ";

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
                    !string.IsNullOrEmpty(vm.ISIN) && vm.HowMany > 0 && !string.IsNullOrEmpty(vm.PricePerShares) && vm.Brokerage > 0)
                {
                    try
                    {
                        SharesPurchaseds dbModel = Get(db, vm.ISIN);

                        if (dbModel != null)
                        {
                            dbModel.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
                            dbModel.CompanyName = vm.CompanyName;
                            dbModel.HowMany = vm.HowMany;
                            dbModel.PricePerShares = double.Parse(vm.PricePerShares);
                            dbModel.Brokerage = vm.Brokerage;
                            dbModel.Currency = vm.Currency;
                            dbModel.ISIN = vm.ISIN;
                            dbModel.Account = vm.Account;
                            dbModel.Amount = vm.HowMany * double.Parse(vm.PricePerShares);
                            dbModel.TypeOfShares = vm.TypeOfShares;
                            dbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Aktien hittades inte i databasen!";
                    }
                    catch (Exception ex)
                    {
                        return $"Ändra köp: {ex.Message}";
                    }
                }
                else
                    return "Du måste fylla i fälten: Företag, ISIN, Inköpsdatum, Antal, Pris per aktie och Courage!";
            }
            else
                return "Aktien hittades inte i databasen eller saknas data i formuläret!";

            return string.Empty;
        }

        public string AddMore(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import)
        {
            if (vm != null && !string.IsNullOrEmpty(vm.ISIN) && db != null)
            {
                if (import == false)
                    if (vm.MoreDateOfPurchase == DateTime.MinValue || vm.MoreHowMany == 0 || vm.MorePricePerShares == 0 || vm.MoreBrokerage == 0)
                        return "Du måste fylla i fälten: Köp mer: Datum, Köp mer: Antal, Köp mer: Pris per aktie, Köp mer: Courage!";

                string importTrue = import ? "Ja" : "Nej";

                SharesPurchaseds DbModel = Get(db, vm.ISIN);

                if (DbModel != null)
                {
                    if (import)
                    {
                        DbModel.HowMany += vm.HowMany;
                        DbModel.Brokerage += vm.Brokerage;
                        DbModel.Amount += vm.HowMany * double.Parse(vm.PricePerShares);
                        DbModel.Note += $"\r\n\r\nKöper mer:" +
                                        $"\r\nBolag: {vm.CompanyName}" +
                                        $"\r\nDatum: {vm.DateOfPurchase.ToString()[..10]} " +
                                        $"\r\nHur många: {vm.HowMany}" +
                                        $"\r\nPris per st: {vm.PricePerShares} " +
                                        $"\r\nInköpsvärdet: {vm.MorePricePerShares * vm.MoreHowMany}" +
                                        $"\r\nTotala värdet: {DbModel.Amount}  " +
                                        $"\r\nCourtage: {vm.Brokerage} " +
                                        $"\r\nImport: {importTrue} ";
                    }
                    else
                    {
                        DbModel.HowMany += vm.MoreHowMany;
                        DbModel.Brokerage += vm.MoreBrokerage;
                        DbModel.Amount += vm.MoreHowMany * vm.MorePricePerShares;
                        DbModel.Note += $"\r\n\r\nKöper mer:" +
                                        $"\r\nBolag: {vm.CompanyName}" +
                                        $"\r\nDatum: {vm.MoreDateOfPurchase.ToString()[..10]}" +
                                        $"\r\nHur många: {vm.MoreHowMany}" +
                                        $"\r\nPris per st: {vm.MorePricePerShares}" +
                                        $"\r\nInköpsvärdet: {vm.MorePricePerShares * vm.MoreHowMany}" +
                                        $"\r\nTotala värdet: {DbModel.Amount}  " +
                                        $"\r\nCourtage: {vm.MoreBrokerage} " +
                                        $"\r\nImport: {importTrue} ";
                    }

                    DbModel.PricePerShares = DbModel.Amount / DbModel.HowMany;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        

                        if (import)
                            ErrorHandling(db, vm, "Köpt mera", import, ex.Message);
                        else
                            return $"Köpt mera. Felmeddelande: {ex.Message}";
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Köpt mera", import, "Hittar inte aktien i databasen!");
                    else
                        return "Hittar inte aktien i databasen!";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Köpt mera", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
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
                {
                    if (vm.SaleDateOfPurchase == DateTime.MinValue && vm.SaleHowMany > 0 && vm.SalePricePerShares > 0 && vm.SaleBrokerage > 0)
                        return "Du måste fylla i fälten: Sälj: Datum, Sälj: Antal, Sälj: Pris per aktie, Sälj: Courage!";
                }
                else if (import == true && vm.SaleBrokerage == 0)
                {
                    ErrorHandling(db, vm, "Sälj", import, "Du får inte sälja aktien utan courage avgift!");
                    return string.Empty;
                }

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
                            Note = $"{getDbSharesPurchasedsModel.Note}" +
                                   $"\r\n\r\nSåld: " +
                                   $"\r\nBolag: {getDbSharesPurchasedsModel.CompanyName}" +
                                   $"\r\nDatum: {vm.SaleDateOfPurchase.ToString()[..10]} " +
                                   $"\r\nHur många: {vm.SaleHowMany}" +
                                   $"\r\nPris per st: {vm.SalePricePerShares} " +
                                   $"\r\nSälj värdet: {vm.SaleHowMany * vm.SalePricePerShares}" +
                                   $"\r\nCourtage: {vm.SaleBrokerage} " +
                                   $"\r\nTotal courtage: {getDbSharesPurchasedsModel.Brokerage + vm.SaleBrokerage} " +
                                   $"\r\nImport: {importTrue} "
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
                        SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(vm, shares.Brokerage, $"Courtage för aktien: \r\nBolag: {vm.CompanyName} \r\nISIN: {vm.ISIN}");
                        sharesFeeClass.Add(db, FeeVM, import, shares.DateOfSold);

                        // Removes the bought shares that is moved to sold shares
                        Delete(db, getDbSharesPurchasedsModel, vm, import);
                    }

                    // Sells parts of the share
                    else
                    {
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
                            Note = $"{getDbSharesPurchasedsModel.Note} " +
                                   $"\r\n\r\nSålt delar: " +
                                   $"\r\nBolag: {getDbSharesPurchasedsModel.CompanyName}" +
                                   $"\r\nDatum: {vm.SaleDateOfPurchase.ToString()[..10]}" +
                                   $"\r\nHur många: {vm.SaleHowMany}" +
                                   $"\r\nPris per st: {vm.SalePricePerShares} " +
                                   $"\r\nSälj värdet: {vm.SaleHowMany * vm.SalePricePerShares}" +
                                   $"\r\nCourtage: {vm.SaleBrokerage} " +
                                   $"\r\nImport: {importTrue} "
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
                        SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(vm, shares.Brokerage, $"Courtage för sålda delar av aktien: \r\nBolag: {vm.CompanyName} \r\nISIN: {vm.ISIN}");
                        sharesFeeClass.Add(db, FeeVM, false, shares.DateOfSold);

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
                    double howManyLeft = dbModel.HowMany - vm.SaleHowMany;

                    double residualValue = howManyLeft * double.Round(dbModel.PricePerShares, 2, MidpointRounding.AwayFromZero);

                    dbModel.HowMany -= vm.SaleHowMany;
                    dbModel.Amount = residualValue;

                    if (string.IsNullOrEmpty(dbModel.Note))
                    {
                        dbModel.Note = dbModel.Note + $"Sålt delar: " +
                                                      $"\r\nBolag: {vm.CompanyName} " +
                                                      $"\r\nDatum: {vm.SaleDateOfPurchase.ToString()[..10]} " +
                                                      $"\r\nImport: {importTrue} " +
                                                      $"\r\nHur många: {vm.SaleHowMany} " +
                                                      $"\r\nPris per st: {vm.SalePricePerShares}" +
                                                      $"\r\nSälj värdet: {vm.SaleHowMany * vm.SalePricePerShares} " +
                                                      $"\r\nKvarvarande värde: {residualValue} " +
                                                      $"\r\nCourtage: {vm.SaleBrokerage} " +
                                                      $"\r\nImport: {importTrue} ";
                    }
                    else
                    {
                        dbModel.Note = dbModel.Note + $"Sålt delar: " +
                                                      $"\r\nBolag: {vm.CompanyName} " +
                                                      $"\r\nDatum: {vm.SaleDateOfPurchase.ToString()[..10]} " +
                                                      $"\r\nHur många: {vm.SaleHowMany}" +
                                                      $"\r\nPris per st: {vm.SalePricePerShares} " +
                                                      $"\r\nSälj värdet: {vm.SaleHowMany * vm.SalePricePerShares}" +
                                                      $"\r\nKvarvarande värde: {residualValue}" +
                                                      $"\r\nCourtage: {vm.SaleBrokerage} " +
                                                      $"\r\nImport: {importTrue} ";
                    }

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
            if (vm != null && vm.SharesPurchasedId != 0 && db != null)
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
                PricePerShares = double.Round(model.PricePerShares, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                Brokerage = model.Brokerage,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Amount = double.Round(model.Amount, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                TypeOfShares = model.TypeOfShares,
                Note = model.Note
            };

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
                if (model.AmountString.Contains("-"))
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

        private static SharesPurchaseds ChangesFromViewModelToModel(SharesPurchasedViewModel vm)
        {
            SharesPurchaseds sharesPurchased = new()
            {
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = double.Round(double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero),
                Brokerage = double.Round(vm.Brokerage, 2, MidpointRounding.AwayFromZero),
                Amount = double.Round(vm.HowMany * double.Parse(vm.PricePerShares), 2, MidpointRounding.AwayFromZero),
                ISIN = vm.ISIN,
                Currency = vm.Currency,
                Account = vm.Account,
                TypeOfShares = vm.TypeOfShares,
                Note = vm.Note
            };

            return sharesPurchased;
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

        private static void ErrorHandling(ApplicationDbContext db, SharesPurchasedViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

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

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}