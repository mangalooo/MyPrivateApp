using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedClass : ISharesPurchasedClass
    {
        private static SharesPurchaseds? Get(ApplicationDbContext db, string ISIN) => db.SharesPurchaseds.Any(r => r.ISIN == ISIN) ?
                                                                                            db.SharesPurchaseds.FirstOrDefault(r => r.ISIN == ISIN) :
                                                                                                throw new Exception("Den köpta aktien hittades inte i databasen!");

        public void Add(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            if (vm.SharesPurchasedId != 0) return;

            SharesPurchaseds model = ChangesFromViewModelToModel(vm);
            model.Note += $"Import: {importTrue}, Köper {model.CompanyName} aktier, Datum: {model.DateOfPurchase.ToString()[..10]}, Hur många: {model.HowMany} " +
                          $"Pris per st: {model.PricePerShares}, Summan: {model.HowMany * model.PricePerShares}, Courtage: {model.Brokerage}. ";

            try
            {
                db.SharesPurchaseds.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create shares {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Köp: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public void Edit(ApplicationDbContext db, SharesPurchasedViewModel vm)
        {
            if (vm.SharesPurchasedId > 0)
            {
                SharesPurchaseds dbModel = Get(db, vm.ISIN);
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

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EditPurchasedShares {DateTime.Now}: Company: {vm.CompanyName} Date: {vm.DateOfPurchase} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: Nej, Ändra köp: {DateTime.Now}: Företag: {vm.CompanyName} Datum: {vm.DateOfPurchase}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            } 
        }

        public void AddMore(ApplicationDbContext db, SharesPurchasedViewModel moreSharesPurchased, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            SharesPurchaseds getDbSharesPurchasedsModel = Get(db, moreSharesPurchased.ISIN);

            getDbSharesPurchasedsModel.HowMany += moreSharesPurchased.MoreHowMany;
            getDbSharesPurchasedsModel.Brokerage += moreSharesPurchased.MoreBrokerage;
            getDbSharesPurchasedsModel.Amount += moreSharesPurchased.MoreHowMany * moreSharesPurchased.MorePricePerShares;
            getDbSharesPurchasedsModel.PricePerShares = getDbSharesPurchasedsModel.Amount / getDbSharesPurchasedsModel.HowMany;
            getDbSharesPurchasedsModel.Note += $" |*** Import: {importTrue}, Köper mer aktier för {moreSharesPurchased.CompanyName}: Datum: " +
                $"{moreSharesPurchased.MoreDateOfPurchase.ToString()[..10]}, Hur många: {moreSharesPurchased.MoreHowMany}, Pris per st: " +
                $"{moreSharesPurchased.MorePricePerShares}, Summan: {moreSharesPurchased.MoreHowMany * moreSharesPurchased.MorePricePerShares}, " +
                $"Courtage: {moreSharesPurchased.MoreBrokerage}. ";

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddMoreShares {DateTime.Now}: Company: {moreSharesPurchased.CompanyName} Date: {moreSharesPurchased.MoreDateOfPurchase.ToString()[..10]} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Köp mera: {DateTime.Now}: Företag: {moreSharesPurchased.CompanyName} Datum: {moreSharesPurchased.MoreDateOfPurchase.ToString()[..10]}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        // Selling all or part of the share
        public void Sell(ApplicationDbContext db, SharesPurchasedViewModel vm, bool import, ISharesFeeClass sharesFeeClass)
        {
            string importTrue = import ? "Ja" : "Nej";

            SharesPurchaseds getDbSharesPurchasedsModel = Get(db, vm.ISIN);

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
                    Console.WriteLine($"SoldSharesAdd {DateTime.Now}: Company: {getDbSharesPurchasedsModel.CompanyName} Date: {vm.SaleDateOfPurchase.ToString()[..10]} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Såld lägg till: {DateTime.Now}: Företag: {getDbSharesPurchasedsModel.CompanyName} Datum: {vm.SaleDateOfPurchase.ToString()[..10]}. "
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
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
                    Console.WriteLine($"Sold same shares {DateTime.Now}: Company: {getDbSharesPurchasedsModel.CompanyName} Date: {vm.SaleDateOfPurchase.ToString()[..10]} " +
                                      $"Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Import: {importTrue}, Sålt delar av aktien: {DateTime.Now}: Företag: {getDbSharesPurchasedsModel.CompanyName} Datum: {vm.SaleDateOfPurchase.ToString()[..10]}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                // Brokerage must be added to the fee table! (For the parts that were sold)
                SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(shares.Brokerage, $"Courtage för sålda delar av aktien: {vm.CompanyName}");
                sharesFeeClass.Add(db, FeeVM, false);

                // Removes portions of the purchased shares that are moved to sold shares
                EditSell(db, getDbSharesPurchasedsModel,  vm, import);
            }
        }

        // Removes portions of the purchased shares that are moved to sold shares
        private static void EditSell(ApplicationDbContext db, SharesPurchaseds dbModel, SharesPurchasedViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            try
            {
                if (dbModel != null)
                {
                    dbModel.HowMany -= vm.SaleHowMany;
                    dbModel.Amount = dbModel.HowMany * vm.PricePerShares;
                    dbModel.Note = vm.Note + $"|*** Import: {importTrue},  Sålt delar av aktien  {vm.CompanyName}: " +
                        $"Datum: {vm.SaleDateOfPurchase.ToString()[..10]} Hur många: {vm.SaleHowMany} " +
                        $"Pris per st: {vm.SalePricePerShares} Summan:  {vm.SaleHowMany * vm.SalePricePerShares}, " +
                        $"Courtage: {vm.SaleBrokerage}  ";

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete sold shares: {DateTime.Now}: Company: {dbModel.CompanyName} Date: {vm.SaleDateOfPurchase.ToString()[..10]} " +
                                  $"Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Radera sålda aktier: {DateTime.Now}: Företag: {dbModel.CompanyName} Datum: {vm.SaleDateOfPurchase.ToString()[..10]}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public void Delete(ApplicationDbContext db, SharesPurchaseds incomingModel, SharesPurchasedViewModel vm, bool import)
        {
            string importTrue = import ? "Ja" : "Nej";

            SharesPurchaseds model = ChangesFromViewModelToModel(vm);

            try
            {
                db.ChangeTracker.Clear();

                if (import)
                    db.SharesPurchaseds.Remove(incomingModel);
                else
                    db.SharesPurchaseds.Remove(model);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete purchased {DateTime.Now}: Company: {incomingModel.CompanyName} Date: {incomingModel.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Import: {importTrue}, Ta bort: {DateTime.Now}: Företag: {incomingModel.CompanyName} Datum: {incomingModel.DateOfPurchase}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
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
    }
}