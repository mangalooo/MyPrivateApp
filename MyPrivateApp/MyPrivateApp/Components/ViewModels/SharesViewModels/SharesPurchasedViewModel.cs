﻿
using System.ComponentModel.DataAnnotations;


namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesPurchasedViewModel
    {
        public SharesPurchasedViewModel() => DateOfPurchase = DateTime.Now;

        public int SharesPurchasedId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; }

        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? CompanyName { get; set; }

        [Display(Name = "Antal")]
        [DataType(DataType.Text)]
        public double HowMany { get; set; }

        [Display(Name = "Pris per aktie")]
        [DataType(DataType.Text)]
        public string? PricePerShares { get; set; }

        [Display(Name = "Courtage")]
        [DataType(DataType.Text)]
        public double Brokerage { get; set; }

        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Text)]
        public string? Amount { get; set; }

        [Display(Name = "Typ av aktie")]
        [DataType(DataType.Text)]
        public string? TypeOfShares { get; set; }

        [Display(Name = "Valuta")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Display(Name = "ISIN")]
        [DataType(DataType.Text)]
        public string? ISIN { get; set; }

        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? Account { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // Add more
        [Display(Name = "Köp mer: Datum")]
        [DataType(DataType.Date)]
        public DateTime MoreDateOfPurchase { get; set; }

        [Display(Name = "Köp mer: Antal")]
        public double MoreHowMany { get; set; }

        [Display(Name = "Köp mer: Pris per aktie")]
        [DataType(DataType.Currency)]
        public double MorePricePerShares { get; set; }

        [Display(Name = "Köp mer: Courtage")]
        [DataType(DataType.Currency)]
        public double MoreBrokerage { get; set; }

        // Sale
        [Display(Name = "Sälj: Datum")]
        [DataType(DataType.Date)]
        public DateTime SaleDateOfPurchase { get; set; }

        [Display(Name = "Sälj: Antal")]
        public double SaleHowMany { get; set; }

        [Display(Name = "Sälj: Pris per aktie")]
        [DataType(DataType.Currency)]
        public double SalePricePerShares { get; set; }

        [Display(Name = "Sälj: Courtage")]
        [DataType(DataType.Currency)]
        public double SaleBrokerage { get; set; }
    }
}