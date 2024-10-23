using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesSoldViewModel
    {
        public SharesSoldViewModel() => DateOfSold = DateTime.Now;

        public int SharesSoldId { get; set; }

        [Display(Name = "Köpsdatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; }

        [Required(ErrorMessage = "Skriv in när aktien såldes")]
        [Display(Name = "Säljsdatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfSold { get; set; }

        [Required(ErrorMessage = "Shares_Error_Amount")]
        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Text)]
        public string? Amount { get; set; }

        [Required(ErrorMessage = "Skriv in vad aktien kostade totalt")]
        [Display(Name = "Säljvärdet")]
        [DataType(DataType.Text)]
        public string? AmountSold { get; set; }

        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? CompanyName { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Typ av aktie")]
        [DataType(DataType.Text)]
        public string? TypeOfShares { get; set; }

        [Display(Name = "Antal")]
        public int HowMany { get; set; }

        [Required(ErrorMessage = "Skriv in vad aktierna kostade per styck")]
        [Display(Name = "Pris per aktie")]
        [DataType(DataType.Text)]
        public string? PricePerShares { get; set; }

        [Required(ErrorMessage = "Skriv in vad aktierna såldes per styck")]
        [Display(Name = "Pris per såld aktie")]
        [DataType(DataType.Text)]
        public string? PricePerSharesSold { get; set; }

        [Display(Name = "Vinst/förlust")]
        [DataType(DataType.Text)]
        public string? MoneyProfitOrLoss { get; set; }

        [Display(Name = "Vinst/förlust i %")]
        public string? PercentProfitOrLoss { get; set; }

        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double Brokerage { get; set; }

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