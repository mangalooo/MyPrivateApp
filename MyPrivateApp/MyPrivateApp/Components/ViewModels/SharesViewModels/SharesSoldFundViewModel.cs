using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesSoldFundViewModel
    {
        public SharesSoldFundViewModel() => DateOfSold = DateTime.Now;

        public int SharesSoldFundId { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Köpsdatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; }

        [Required(ErrorMessage = "Skriv in när aktien såldes")]
        [Display(Name = "Säljsdatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfSold { get; set; }

        [Required(ErrorMessage = "Shares_Error_Amount")]
        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Skriv in vad aktien kostade totalt")]
        [Display(Name = "Säljvärdet")]
        [DataType(DataType.Currency)]
        public double AmountSold { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Fond namn")]
        [DataType(DataType.Text)]
        public string? FundName { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Typ av fond")]
        [DataType(DataType.Text)]
        public string? TypeOfFund { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Antal")]
        public double HowMany { get; set; }

        [Required(ErrorMessage = "Skriv in vad fonden kostade per styck")]
        [Display(Name = "Pris per fond")]
        [DataType(DataType.Currency)]
        public double PricePerFunds{ get; set; }

        [Required(ErrorMessage = "Skriv in vad fonden såldes per styck")]
        [Display(Name = "Pris per såld fond")]
        [DataType(DataType.Currency)]
        public double PricePerFundsSold { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Vinst/förlust")]
        [DataType(DataType.Currency)]
        public double MoneyProfitOrLoss { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Vinst/förlust i %")]
        public string? PercentProfitOrLoss { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Avgift")]
        [DataType(DataType.Currency)]
        public double Fee { get; set; }

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