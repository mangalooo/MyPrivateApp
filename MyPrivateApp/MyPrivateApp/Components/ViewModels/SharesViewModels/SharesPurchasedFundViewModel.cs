using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesPurchasedFundViewModel
    {
        public SharesPurchasedFundViewModel() => DateOfPurchase = DateTime.Now;

        public int SharesPurchasedFundId { get; set; }

        [Required(ErrorMessage = "Skriv in datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; }

        [Required(ErrorMessage = "Skriv in ett fond namn")]
        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? FundName { get; set; }

        [Required(ErrorMessage = "Skriv hur många fond delar")]
        [Display(Name = "Antal")]
        public double HowMany { get; set; }

        [Required(ErrorMessage = "Skriv in pris per fond del")]
        [Display(Name = "Pris per aktie")]
        [DataType(DataType.Currency)]
        public double PricePerFunds { get; set; }

        [Required(ErrorMessage = "Skriv in avgift")]
        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double Fee { get; set; }

        [Required(ErrorMessage = "Skriv in vad fonden kostade totalt")]
        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }

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
        public int MoreHowMany { get; set; }

        [Display(Name = "Köp mer: Pris per fond del")]
        [DataType(DataType.Currency)]
        public double MorePricePerFunds { get; set; }

        [Display(Name = "Köp mer: Avgift")]
        [DataType(DataType.Currency)]
        public int MoreFee { get; set; }

        // Sale
        [Display(Name = "Sälj: Datum")]
        [DataType(DataType.Date)]
        public DateTime SaleDateOfPurchase { get; set; }

        [Display(Name = "Sälj: Antal")]
        public int SaleHowMany { get; set; }

        [Display(Name = "Sälj: Pris per fond del")]
        [DataType(DataType.Currency)]
        public double SalePricePerFunds { get; set; }

        [Display(Name = "Sälj: Courtage")]
        [DataType(DataType.Currency)]
        public double SaleFee { get; set; }
    }
}