﻿using MyPrivateApp.Components.ViewModels.SharesViewModels;
using MyPrivateApp.Data.Models.SharesModels;
using MyPrivateApp.Data;

namespace MyPrivateApp.Components.Shares.Classes
{
    public class SharesDividendClass : ISharesDividendClass
    {
        private static SharesDividend? Get(ApplicationDbContext db, int id) => db.SharesDividends.Any(r => r.DividendId == id) ?
                                                                                        db.SharesDividends.FirstOrDefault(r => r.DividendId == id) :
                                                                                            throw new Exception("Den köpa aktien hittades inte i databasen!");

        public string Add(ApplicationDbContext db, SharesDividendViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Company) && !string.IsNullOrEmpty(vm.ISIN) 
                    && vm.NumberOfShares > 0 && !string.IsNullOrEmpty(vm.PricePerShare))
                {
                    try
                    {
                        SharesDividend model = ChangeFromViewModelToModel(vm);

                        db.SharesDividends.Add(model);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Lägg till", import, ex.Message);
                    }
                }
                else
                {
                    if (import)
                        ErrorHandling(db, vm, "Lägg till", import, "Du måste fylla i fälten: Inköpsdatum, Företag, ISIN, Antal och Pris per aktie. ");
                    else
                        return "Du måste fylla i fälten: Inköpsdatum, Företag, ISIN, Antal och Pris per aktie. ";
                }
            }
            else
            {
                if (import)
                    ErrorHandling(db, vm, "Lägg till", import, "Hittar ingen data från formuläret eller ingen kontakt med databasen!");
                else
                    return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";
            }

            return string.Empty;
        }

        public string Edit(ApplicationDbContext db, SharesDividendViewModel vm, bool import)
        {
            if (vm != null && db != null)
            {
                if (vm.DividendId > 0 && vm.Date != DateTime.MinValue && !string.IsNullOrEmpty(vm.Company) &&
                    !string.IsNullOrEmpty(vm.ISIN) && vm.NumberOfShares > 0 && string.IsNullOrEmpty(vm.PricePerShare))
                {
                    try
                    {
                        SharesDividend dbModel = Get(db, vm.DividendId);

                        if (dbModel != null)
                        {

                            dbModel.Date = vm.Date.ToString("yyyy-MM-dd");
                            dbModel.AccountNumber = vm.AccountNumber;
                            dbModel.TypeOfTransaction = vm.TypeOfTransaction;
                            dbModel.Company = vm.Company;
                            dbModel.NumberOfShares = vm.NumberOfShares;
                            dbModel.PricePerShare = double.Parse(vm.PricePerShare);
                            dbModel.TotalAmount = vm.NumberOfShares * double.Parse(vm.PricePerShare);
                            dbModel.Currency = vm.Currency;
                            dbModel.ISIN = vm.ISIN;
                            dbModel.Note = vm.Note;

                            db.SaveChanges();
                        }
                        else
                            return "Hittar inte aktien i databasen!";
                    }
                    catch (Exception ex)
                    {
                        ErrorHandling(db, vm, "Ändra", import, ex.Message);
                    }
                }
                else
                    return "Du måste fylla i fälten: Inköpsdatum, Företag, ISIN, Antal och Pris per aktie. ";
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public string Delete(ApplicationDbContext db, SharesDividendViewModel vm, bool import)
        {
            if (vm != null && vm.DividendId > 0 && db != null)
            {
                try
                {
                    SharesDividend model = ChangeFromViewModelToModel(vm);

                    db.ChangeTracker.Clear();
                    db.SharesDividends.Remove(model);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandling(db, vm, "Ta bort", import, ex.Message);
                }
            }
            else
                return "Hittar ingen data från formuläret eller ingen kontakt med databasen!";

            return string.Empty;
        }

        public SharesDividendViewModel ChangeFromModelToViewModel(SharesDividend model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesDividendViewModel vm = new()
            {
                DividendId = model.DividendId,
                Date = date,
                AccountNumber = model.AccountNumber,
                TypeOfTransaction = model.TypeOfTransaction,
                Company = model.Company,
                NumberOfShares = model.NumberOfShares,
                PricePerShare = model.PricePerShare.ToString("#,##0.00"),
                TotalAmount = model.TotalAmount.ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN,
                Note = model.Note
            };

            return vm;
        }

        public SharesDividendViewModel ChangeFromImportToViewModel(SharesImports model)
        {
            DateTime date = DateTime.Parse(model.Date);

            SharesDividendViewModel vm = new()
            {
                Date = date,
                Company = model.CompanyOrInformation,
                NumberOfShares = int.Parse(model.NumberOfSharesString),
                PricePerShare = double.Parse(model.PricePerShareString).ToString("#,##0.00"),
                Currency = model.Currency,
                ISIN = model.ISIN,
                AccountNumber = model.AccountNumber,
                TotalAmount = double.Parse(model.AmountString).ToString("#,##0.00"),
                TypeOfTransaction = model.TypeOfTransaction
            };

            return vm;
        }

        private static SharesDividend ChangeFromViewModelToModel(SharesDividendViewModel vm)
        {
            SharesDividend sharesDividend = new()
            {
                Date = vm.Date.ToString("yyyy-MM-dd"),
                AccountNumber = vm.AccountNumber,
                TypeOfTransaction = vm.TypeOfTransaction,
                Company = vm.Company,
                NumberOfShares = vm.NumberOfShares,
                PricePerShare = double.Parse(vm.PricePerShare),
                TotalAmount = double.Parse(vm.PricePerShare) * vm.NumberOfShares,
                Currency = vm.Currency,
                ISIN = vm.ISIN,
                Note = vm.Note
            };

            return sharesDividend;
        }

        private static void ErrorHandling(ApplicationDbContext db, SharesDividendViewModel vm, string type, bool import, string errorMessage)
        {
            DateTime date = DateTime.Now;
            string importTrue = import ? "Ja" : "Nej";

            SharesErrorHandlings sharesErrorHandling = new()
            {
                Date = $"{date.Year}-{date.Month}-{date.Day}",
                CompanyOrInformation = vm.Company,
                TypeOfTransaction = vm.TypeOfTransaction,
                ErrorMessage = $"Felmeddelande: {errorMessage}",
                Note = $"{type} UTDELNING: \r\nDatum: {vm.Date} \r\nImport: {importTrue}  \r\nISIN: {vm.ISIN} \r\nId: {vm.DividendId}. "
            };

            db.SharesErrorHandlings.Add(sharesErrorHandling);
            db.SaveChanges();
        }
    }
}