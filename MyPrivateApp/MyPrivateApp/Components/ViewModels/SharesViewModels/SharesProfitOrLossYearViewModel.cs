using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesProfitOrLossYearViewModel
    {
        [Key]
        public int SharesProfitOrLossYearsId { get; set; }

        [Display(Name = "År")]
        [DataType(DataType.Text)]
        public string? Year { get; set; }

        [Display(Name = "Aktier")]
        [DataType(DataType.Text)]
        public string? SharesYear { get; set; }

        [Display(Name = "Fonder")]
        [DataType(DataType.Text)]
        public string? FundsYear { get; set; }

        [Display(Name = "Utdelning")]
        [DataType(DataType.Text)]
        public string? DividendYear { get; set; }

        [Display(Name = "Ränta")]
        [DataType(DataType.Text)]
        public string? InterestRatesYear { get; set; }

        [Display(Name = "Skatt")]
        [DataType(DataType.Text)]
        public string? FeeYear { get; set; }

        [Display(Name = "Skatt")]
        [DataType(DataType.Text)]
        public string? TaxYear { get; set; }

        [Display(Name = "Courtage")]
        [DataType(DataType.Text)]
        public string? BrokerageYear { get; set; }

        [Display(Name = "Vinst/förlust")]
        [DataType(DataType.Text)]
        public string? MoneyProfitOrLossYear { get; set; }

        [Display(Name = "Vinst/förlust i %")]
        [DataType(DataType.Text)]
        public string? PercentProfitOrLossYear { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.Text)]
        public string? Note { get; set; }
    }
}