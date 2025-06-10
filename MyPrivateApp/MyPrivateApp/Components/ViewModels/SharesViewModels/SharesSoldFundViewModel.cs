
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

        [Display(Name = "Säljsdatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfSold { get; set; }

        [Display(Name = "Inköpsvärdet")]
        [DataType(DataType.Text)]
        public string? Amount { get; set; }

        [Display(Name = "Säljvärdet")]
        [DataType(DataType.Text)]
        public string? AmountSold { get; set; }

        [Display(Name = "Fond namn")]
        [DataType(DataType.Text)]
        public string? FundName { get; set; }

        [Display(Name = "Typ av fond")]
        [DataType(DataType.Text)]
        public string? TypeOfFund { get; set; }

        [Display(Name = "Antal")]
        [DataType(DataType.Text)]
        public double HowMany { get; set; }

        [Display(Name = "Pris per fond")]
        [DataType(DataType.Text)]
        public string? PricePerFunds{ get; set; }

        [Display(Name = "Pris per såld fond")]
        [DataType(DataType.Text)]
        public string? PricePerFundsSold { get; set; }

        [Display(Name = "Vinst/förlust")]
        [DataType(DataType.Text)]
        public string? MoneyProfitOrLoss { get; set; }

        [Display(Name = "Vinst/förlust i %")]
        [DataType(DataType.Text)]
        public string? PercentProfitOrLoss { get; set; }

        [Display(Name = "Avgift")]
        [DataType(DataType.Text)]
        public double Fee { get; set; }

        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
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