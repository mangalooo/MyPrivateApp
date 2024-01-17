using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data;
using MyPrivateApp.Data.Models.SharesModels;

namespace MyPrivateApp.Components.Shares.HelpClasses
{
    public class SharesPurchasedHelpClass
    {
        private static SharesPurchaseds GetSharesPurchased(ApplicationDbContext db, int? id) => db.SharesPurchaseds.FirstOrDefault(r => r.SharesPurchasedId == id);

        public static void CreateOrUpdatePurchasedShares(ApplicationDbContext db, SharesPurchasedViewModel sharesPurchased)
        {
            // Update hunting
            if (sharesPurchased.SharesPurchasedId > 0)
            {
                SharesPurchaseds c = db.SharesPurchaseds.FirstOrDefault(p => p.SharesPurchasedId == sharesPurchased.SharesPurchasedId);

                if (c is null) return;

                SharesPurchaseds model = SetChanges(db, sharesPurchased);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EditPurchasedShares {DateTime.Now}: Company: {model.CompanyName} Date: {model.DateOfPurchase} Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Ändra köp: {DateTime.Now}: Företag: {model.CompanyName} Datum: {model.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                return;
            }

            if (sharesPurchased.SharesPurchasedId != 0) return;

            // Add new hunting
            SharesPurchaseds changesToModel = ChangesSharesPurchasedToModel(sharesPurchased);
            changesToModel.Note += "Köper " + changesToModel.CompanyName + " aktier: " + "Datum: " + changesToModel.DateOfPurchase.ToString().Substring(0, 10) + ", Hur många: " + changesToModel.HowMany +
                        ", Pris per st: " + changesToModel.PricePerShares + ", Summan: " + changesToModel.HowMany * changesToModel.PricePerShares + ", Courtage: " + changesToModel.Brokerage + ". ";

            try
            {
                db.SharesPurchaseds.Add(changesToModel);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddShares {DateTime.Now}: Company: {changesToModel.CompanyName} Date: {changesToModel.DateOfPurchase} Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Köp: {DateTime.Now}: Företag: {changesToModel.CompanyName} Datum: {changesToModel.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }

            return;
        }

        public static SharesPurchaseds SetChanges(ApplicationDbContext db, SharesPurchasedViewModel vm)
        {
            SharesPurchaseds getDbSharesPurchasedsModel = GetSharesPurchased(db, vm.SharesPurchasedId);
            getDbSharesPurchasedsModel.DateOfPurchase = vm.DateOfPurchase.ToString("yyyy-MM-dd");
            getDbSharesPurchasedsModel.CompanyName = vm.CompanyName;
            getDbSharesPurchasedsModel.HowMany = vm.HowMany;
            getDbSharesPurchasedsModel.PricePerShares = vm.PricePerShares;
            getDbSharesPurchasedsModel.Brokerage = vm.Brokerage;
            getDbSharesPurchasedsModel.Currency = vm.Currency;
            getDbSharesPurchasedsModel.ISIN = vm.ISIN;
            getDbSharesPurchasedsModel.Account = vm.Account;
            getDbSharesPurchasedsModel.Amount = vm.HowMany * vm.PricePerShares;
            getDbSharesPurchasedsModel.TypeOfShares = vm.TypeOfShares;
            getDbSharesPurchasedsModel.Note = vm.Note;

            return getDbSharesPurchasedsModel;
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

        public static SharesPurchaseds ChangesSharesPurchasedToModel(SharesPurchasedViewModel vm)
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

        public static void DeleteSharesPurchased(ApplicationDbContext db, SharesPurchasedViewModel incomingModel)
        {
            SharesPurchaseds sharesPurchasedModel = ChangesSharesPurchasedToModel(incomingModel);

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
                    ErrorMessage = $"Ta bort: {DateTime.Now}: Företag: {sharesPurchasedModel.CompanyName} Datum: {sharesPurchasedModel.DateOfPurchase} \n Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        public static void AddMoreShares(ApplicationDbContext db, SharesPurchasedViewModel moreSharesPurchased)
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
                    ErrorMessage = $"Köp mera: {DateTime.Now}: Företag: {moreSharesPurchased.CompanyName} Datum: {moreSharesPurchased.MoreDateOfPurchase.ToString()[..10]} \n Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        // Säljer hela eller delar av aktien
        public static void SellPurchasedShares(ApplicationDbContext db, SharesPurchasedViewModel vm)
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
                        ErrorMessage = $"Såld lägg till: {DateTime.Now}: Företag: {vm.CompanyName} Datum: {vm.SaleDateOfPurchase.ToString()[..10]} \n Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                // Brokerage ska läggas på avgift tabellen!
                SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(shares.Brokerage, $"Courtage för aktien: {vm.CompanyName}");
                SharesFeeHelpClass.Create(db, FeeVM);

                // Tar bort den köpte aktien som flyttas till sålda aktier
                DeleteSharesPurchased(db, vm);
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
                    Console.WriteLine($"SoldSharesAdd ELSE {DateTime.Now}: Company: {vm.CompanyName} Date: {vm.SaleDateOfPurchase.ToString()[..10]} " +
                                      $"Error: {ex.Message}");

                    DateTime date = DateTime.Now;

                    SharesErrorHandlings sharesErrorHandling = new()
                    {
                        Date = $"{date.Year}-{date.Month}-{date.Day}",
                        ErrorMessage = $"Såld lägg till annars: {DateTime.Now}: Företag: {vm.CompanyName} Datum: {vm.SaleDateOfPurchase.ToString()[..10]}" +
                                       $"Felmeddelande: {ex.Message}"
                    };

                    db.SharesErrorHandlings.Add(sharesErrorHandling);
                    db.SaveChanges();
                }

                // Brokerage ska läggas på avgift tabellen! (För delarna som såldes)
                SharesFeeViewModel FeeVM = ChangeFromToPurchasedToFeeViewModel(shares.Brokerage, $"Courtage för sålda delar av aktien: {vm.CompanyName}");
                SharesFeeHelpClass.Create(db, FeeVM);

                // Tar bort delar av den köpta aktien som flyttas till sålda aktier
                EditSellShares(db, vm);
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

        private static void EditSellShares(ApplicationDbContext db, SharesPurchasedViewModel saleSharesPurchased)
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
                Console.WriteLine($"EditSellShares {DateTime.Now}: Company: {saleSharesPurchased.CompanyName} Date: {saleSharesPurchased.SaleDateOfPurchase.ToString()[..10]} " +
                                  $"Error: {ex.Message}");

                DateTime date = DateTime.Now;

                SharesErrorHandlings sharesErrorHandling = new()
                {
                    Date = $"{date.Year}-{date.Month}-{date.Day}",
                    ErrorMessage = $"Ändra sälj: {DateTime.Now}: Företag: {saleSharesPurchased.CompanyName} Datum: {saleSharesPurchased.SaleDateOfPurchase.ToString()[..10]} \n " +
                                   $"Felmeddelande: {ex.Message}"
                };

                db.SharesErrorHandlings.Add(sharesErrorHandling);
                db.SaveChanges();
            }
        }

        private static string ConvertToPercentage(double decimalValue) => $"{decimalValue * 100:F2}%";
    }
}