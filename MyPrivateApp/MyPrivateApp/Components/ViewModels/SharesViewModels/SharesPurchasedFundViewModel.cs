using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesPurchasedFundViewModel
    {
        public SharesPurchasedFundViewModel() => DateOfPurchase = DateTime.Now;

        public int SharesPurchasedFundId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; }

        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? FundName { get; set; }

        [Display(Name = "Antal")]
        public double HowMany { get; set; }

        [Display(Name = "Pris per aktie")]
        [DataType(DataType.Text)]
        public string? PricePerFunds { get; set; }

        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double Fee { get; set; }

        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Text)]
        public string? Amount { get; set; }

        [Display(Name = "Typ av fond")]
        [DataType(DataType.Text)]
        public string? TypeOfFund { get; set; }

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

        [Display(Name = "Köp mer: Pris per fond del")]
        [DataType(DataType.Currency)]
        public double MorePricePerFunds { get; set; }

        [Display(Name = "Köp mer: Avgift")]
        [DataType(DataType.Currency)]
        public double MoreFee { get; set; }

        // Sale
        [Display(Name = "Sälj: Datum")]
        [DataType(DataType.Date)]
        public DateTime SaleDateOfPurchase { get; set; }

        [Display(Name = "Sälj: Antal")]
        public double SaleHowMany { get; set; }

        [Display(Name = "Sälj: Pris per fond del")]
        [DataType(DataType.Currency)]
        public double SalePricePerFunds { get; set; }

        [Display(Name = "Sälj: Courtage")]
        [DataType(DataType.Currency)]
        public double SaleFee { get; set; }
    }
}