using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesPurchasedViewModel
    {
        [Key]
        public int SharesPurchasedId { get; set; }

        [Required(ErrorMessage = "Skriv in datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; } //  Köp datum

        [Required(ErrorMessage = "Skriv in ett företagsnamn")]
        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Skriv hur många aktier")]
        [Display(Name = "Antal")]
        public int HowMany { get; set; }

        [Required(ErrorMessage = "Skriv in pris per aktie")]
        [Display(Name = "Pris per aktie")]
        [DataType(DataType.Currency)]
        public double PricePerShares { get; set; }

        [Required(ErrorMessage = "Skriv in courtage")]
        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double Brokerage { get; set; } //courtage, köp kostnad

        [Required(ErrorMessage = "Skriv in vad aktien kostade totalt")]
        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; } // totalbelopp

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
        public DateTime MoreDateOfPurchase { get; set; } //  Köp datum

        [Display(Name = "Köp mer: Antal")]
        public int MoreHowMany { get; set; }

        [Display(Name = "Köp mer: Pris per aktie")]
        [DataType(DataType.Currency)]
        public double MorePricePerShares { get; set; }

        [Display(Name = "Köp mer: Courtage")]
        [DataType(DataType.Currency)]
        public int MoreBrokerage { get; set; } //courtage, köp kostnad

        // Sale
        [Display(Name = "Sälj: Datum")]
        [DataType(DataType.Date)]
        public DateTime SaleDateOfPurchase { get; set; } //  Köp datum

        [Display(Name = "Sälj: Antal")]
        public int SaleHowMany { get; set; }

        [Display(Name = "Sälj: Pris per aktie")]
        [DataType(DataType.Currency)]
        public double SalePricePerShares { get; set; }

        [Display(Name = "Sälj: Courtage")]
        [DataType(DataType.Currency)]
        public double SaleBrokerage { get; set; } //courtage, köp kostnad
    }
}