using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesSoldViewModel
    {
        public SharesSoldViewModel() => DateOfSold = DateTime.Now;

        public int SharesSoldId { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Köpsdatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; } //  Köpsdatum. String? 

        [Required(ErrorMessage = "Skriv in när aktien såldes")]
        [Display(Name = "Säljsdatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfSold { get; set; } //  Säljsdatum. String? 

        [Required(ErrorMessage = "Shares_Error_Amount")]
        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; } // Totalbelopp. Uträkning nytt värde

        [Required(ErrorMessage = "Skriv in vad aktien kostade totalt")]
        [Display(Name = "Säljvärdet")]
        [DataType(DataType.Currency)]
        public double AmountSold { get; set; } // Totalbelopp från solda värdet 

        [ScaffoldColumn(false)]
        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? CompanyName { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Typ av aktie")]
        [DataType(DataType.Text)]
        public string? TypeOfShares { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Antal")]
        public int HowMany { get; set; }

        [Required(ErrorMessage = "Skriv in vad aktierna kostade per styck")]
        [Display(Name = "Pris per aktie")]
        [DataType(DataType.Currency)]
        public double PricePerShares { get; set; } // Uträkning nytt värde

        [Required(ErrorMessage = "Skriv in vad aktierna såldes per styck")]
        [Display(Name = "Pris per såld aktie")]
        [DataType(DataType.Currency)]
        public double PricePerSharesSold { get; set; } // Uträkning nytt värde

        [ScaffoldColumn(false)]
        [Display(Name = "Vinst/förlust")]
        [DataType(DataType.Currency)]
        public double MoneyProfitOrLoss { get; set; } //Vinst eller förlust i pengar. Grön och röd färg?

        [ScaffoldColumn(false)]
        [Display(Name = "Vinst/förlust i %")]
        public string? PercentProfitOrLoss { get; set; } //Vinst eller förlust i %. Grön och röd färg?

        [ScaffoldColumn(false)]
        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double Brokerage { get; set; } //Courtage, köp kostnad

        [Display(Name = "Konto")]
        [DataType(DataType.Currency)]
        public string? Account { get; set; }

        [Display(Name = "Valuta")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Display(Name = "ISIN")]
        [DataType(DataType.Text)]
        public string? ISIN { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}