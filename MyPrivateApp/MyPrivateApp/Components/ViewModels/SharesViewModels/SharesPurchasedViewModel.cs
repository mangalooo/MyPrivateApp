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
        public string? DateOfPurchase { get; set; } //  Köp datum

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
        public int Brokerage { get; set; } //courtage, köp kostnad

        [Required(ErrorMessage = "Skriv in vad aktien kostade totalt")]
        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; } // totalbelopp

        [Display(Name = "Typ av aktie")]
        [DataType(DataType.Text)]
        public string? TypeOfShares { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}