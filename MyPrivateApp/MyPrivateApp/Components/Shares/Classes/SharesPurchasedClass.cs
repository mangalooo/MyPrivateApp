using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesPurchasedHelpClass
    {
        private static SharesPurchaseds GetSharesPurchased(ApplicationDbContext db, int? id) => db.SharesPurchaseds.FirstOrDefault(r => r.SharesPurchasedId == id);

        public static void Update(ApplicationDbContext db, SharesPurchasedViewModel vm)
        {
            // Update hunting
            if (vm.SharesPurchasedId > 0)
            {
                SharesPurchaseds dbModel = GetSharesPurchased(db, vm.SharesPurchasedId);
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
                        Note = $"Ändra köp: {DateTime.Now}: Företag: {vm.CompanyName} Datum: {vm.DateOfPurchase}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }
            } 
        }

        public static void Create(ApplicationDbContext db, SharesPurchasedViewModel vm)
        {
            if (vm.SharesPurchasedId != 0) return;

            // Add new hunting
            SharesPurchaseds model = ChangesFromViewModelToModel(vm);
            model.Note += "Köper " + model.CompanyName + " aktier: " + "Datum: " + model.DateOfPurchase.ToString()[..10] + ", Hur många: " + model.HowMany +
                        ", Pris per st: " + model.PricePerShares + ", Summan: " + model.HowMany * model.PricePerShares + ", Courtage: " + model.Brokerage + ". ";

            try
            {
                db.SharesPurchaseds.Add(model);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddShares {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Köp: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public static SharesPurchasedViewModel ChangeFromModelToViewModel(SharesPurchaseds model)
        {
            DateTime date = DateTime.Parse(model.DateOfPurchase);

            SharesPurchasedViewModel vm = new()
            {
                SharesPurchasedId = model.SharesPurchasedId,
                DateOfPurchase = date,
                CompanyName = model.CompanyName,
                HowMany = model.HowMany,
                PricePerShares = model.PricePerShares,
                Brokerage = model.Brokerage,
                Currency = model.Currency,
                ISIN = model.ISIN,
                Account = model.Account,
                Amount = model.Amount,
                TypeOfShares = model.TypeOfShares,
                Note = model.Note
            };

            return vm;
        }

        public static SharesPurchaseds ChangesFromViewModelToModel(SharesPurchasedViewModel vm)
        {
            SharesPurchaseds sharesPurchased = new()
            {
                SharesPurchasedId = vm.SharesPurchasedId,
                DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                CompanyName = vm.CompanyName,
                HowMany = vm.HowMany,
                PricePerShares = vm.PricePerShares,
                Brokerage = vm.Brokerage,
                Amount = vm.HowMany * vm.PricePerShares,
                ISIN = vm.ISIN,
                Currency = vm.Currency,
                Account = vm.Account,
                TypeOfShares = vm.TypeOfShares,
                Note = vm.Note
            };

            return sharesPurchased;
        }

        public static void Delete(ApplicationDbContext db, SharesPurchasedViewModel incomingModel)
        {
            SharesPurchaseds sharesPurchasedModel = ChangesFromViewModelToModel(incomingModel);

            try
            {
                db.ChangeTracker.Clear();
                db.SharesPurchaseds.Remove(sharesPurchasedModel);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeletePurchased {DateTime.Now}: Company: {sharesPurchasedModel.CompanyName} Date: {sharesPurchasedModel.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Ta bort: {DateTime.Now}: Företag: {sharesPurchasedModel.CompanyName} Datum: {sharesPurchasedModel.DateOfPurchase}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public static void AddMore(ApplicationDbContext db, SharesPurchasedViewModel moreSharesPurchased)
        {
            SharesPurchaseds getDbSharesPurchasedsModel = GetSharesPurchased(db, moreSharesPurchased.SharesPurchasedId);

            getDbSharesPurchasedsModel.HowMany += moreSharesPurchased.MoreHowMany;
            getDbSharesPurchasedsModel.Brokerage += moreSharesPurchased.MoreBrokerage;
            getDbSharesPurchasedsModel.Amount += moreSharesPurchased.MoreHowMany * moreSharesPurchased.MorePricePerShares;
            getDbSharesPurchasedsModel.PricePerShares = getDbSharesPurchasedsModel.Amount / getDbSharesPurchasedsModel.HowMany;
            getDbSharesPurchasedsModel.Note += "| Köper mer aktier för " + moreSharesPurchased.CompanyName + ": " + "Datum: " + moreSharesPurchased.MoreDateOfPurchase.ToString()[..10] + ", " +
                "Hur många: " + moreSharesPurchased.MoreHowMany + ", Pris per st: " + moreSharesPurchased.MorePricePerShares + " Summan: " + moreSharesPurchased.MoreHowMany * moreSharesPurchased.MorePricePerShares +
                " , Courtage: " + moreSharesPurchased.MoreBrokerage + ". ";

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
                    Note = $"Köp mera: {DateTime.Now}: Företag: {moreSharesPurchased.CompanyName} Datum: {moreSharesPurchased.MoreDateOfPurchase.ToString()[..10]}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        // Säljer hela eller delar av aktien
        public static void Sell(ApplicationDbContext db, SharesPurchasedViewModel vm)
        {
            SharesPurchaseds getDbSharesPurchasedsModel = GetSharesPurchased(db, vm.SharesPurchasedId);

            if (getDbSharesPurchasedsModel.HowMany == vm.SaleHowMany)
            {
                SharesSolds shares = new()
                {
                    DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                    DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                    Amount = vm.PricePerShares * vm.SaleHowMany,
                    Brokerage = getDbSharesPurchasedsModel.Brokerage + vm.SaleBrokerage,
                    CompanyName = vm.CompanyName,
                    HowMany = vm.SaleHowMany,
                    TypeOfShares = vm.TypeOfShares,
                    Currency = vm.Currency,
                    ISIN = vm.ISIN,
                    Account = vm.Account,
                    PricePerShares = vm.PricePerShares,
                    PricePerSharesSold = vm.SalePricePerShares,
                    AmountSold = vm.SalePricePerShares * vm.SaleHowMany,
                    Note = $"{vm.Note}| Sålt aktien: {vm.CompanyName}, Datum: {vm.SaleDateOfPurchase.ToString()[..10]}, " +
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
                    Console.WriteLine($"SoldSharesAdd {DateTime.Now}: Company: {vm.CompanyName} Date: {vm.SaleDateOfPurchase.ToString()[..10]} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Såld lägg till: {DateTime.Now}: Företag: {vm.CompanyName} Datum: {vm.SaleDateOfPurchase.ToString()[..10]}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                // Brokerage ska läggas på avgift tabellen!
                SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(shares.Brokerage, $"Courtage för aktien: {vm.CompanyName}");
                SharesFeeClass.Create(db, FeeVM);

                // Tar bort den köpte aktien som flyttas till sålda aktier
                Delete(db, vm);
            }
            else
            {
                SharesSolds shares = new()
                {
                    DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd"),
                    DateOfSold = vm.SaleDateOfPurchase.ToString("yyyy-MM-dd"),
                    Amount = vm.PricePerShares * vm.SaleHowMany,
                    Brokerage = vm.SaleBrokerage,
                    CompanyName = vm.CompanyName,
                    HowMany = vm.SaleHowMany,
                    TypeOfShares = vm.TypeOfShares,
                    Currency = vm.Currency,
                    ISIN = vm.ISIN,
                    Account = vm.Account,
                    PricePerShares = vm.PricePerShares,
                    PricePerSharesSold = vm.SalePricePerShares,
                    AmountSold = vm.SalePricePerShares * vm.SaleHowMany,
                    Note = $"{vm.Note}| Sålt delar av aktien: {vm.CompanyName}, Datum: {vm.SaleDateOfPurchase.ToString()[..10]}, " +
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
                    Console.WriteLine($"Sold same shares {DateTime.Now}: Company: {vm.CompanyName} Date: {vm.SaleDateOfPurchase.ToString()[..10]} " +
                                      $"Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Felmeddelande: {ex.Message}",
                        Note = $"Sålt delar av aktien: {DateTime.Now}: Företag: {vm.CompanyName} Datum: {vm.SaleDateOfPurchase.ToString()[..10]}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                // Brokerage ska läggas på avgift tabellen! (För delarna som såldes)
                SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(shares.Brokerage, $"Courtage för sålda delar av aktien: {vm.CompanyName}");
                SharesFeeClass.Create(db, FeeVM);

                // Tar bort delar av den köpta aktien som flyttas till sålda aktier
                EditSell(db, vm);
            }
        }

        private static SharesFeeViewModel ChangeFromToPurchasedToFeeViewModel(int brokerage, string note)
        {
            SharesFeeViewModel fee = new()
            {
                Date = DateTime.Now,
                Brokerage = brokerage,
                Note = note
            };

            return fee;
        }

        private static void EditSell(ApplicationDbContext db, SharesPurchasedViewModel saleSharesPurchased)
        {
            try
            {
                SharesPurchaseds getDbSharesPurchasedsModel = GetSharesPurchased(db, saleSharesPurchased.SharesPurchasedId);

                if (getDbSharesPurchasedsModel != null)
                {
                    getDbSharesPurchasedsModel.HowMany -= saleSharesPurchased.SaleHowMany;
                    getDbSharesPurchasedsModel.Amount = getDbSharesPurchasedsModel.HowMany * saleSharesPurchased.PricePerShares;
                    getDbSharesPurchasedsModel.Note = saleSharesPurchased.Note + $"| Sålt delar av aktien  {saleSharesPurchased.CompanyName}: " + 
                        $"Datum: {saleSharesPurchased.SaleDateOfPurchase.ToString()[..10]} Hur många: {saleSharesPurchased.SaleHowMany} " +
                        $"Pris per st: {saleSharesPurchased.SalePricePerShares} Summan:  {saleSharesPurchased.SaleHowMany* saleSharesPurchased.SalePricePerShares}, " +
                        $"Courtage: {saleSharesPurchased.SaleBrokerage}  ";

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete sold shares: {DateTime.Now}: Company: {saleSharesPurchased.CompanyName} Date: {saleSharesPurchased.SaleDateOfPurchase.ToString()[..10]} " +
                                  $"Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Felmeddelande: {ex.Message}",
                    Note = $"Radera sålda aktier: {DateTime.Now}: Företag: {saleSharesPurchased.CompanyName} Datum: {saleSharesPurchased.SaleDateOfPurchase.ToString()[..10]}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";
    }
}