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
        [DataType(DataType.Currency)]
        public double SharesYear { get; set; }

        [Display(Name = "Fonder")]
        [DataType(DataType.Currency)]
        public double FundsYear { get; set; }

        [Display(Name = "Utdelning")]
        [DataType(DataType.Currency)]
        public double DividendYear { get; set; }

        [Display(Name = "Ränta")]
        [DataType(DataType.Currency)]
        public double InterestRatesYear { get; set; }

        [Display(Name = "Skatt")]
        [DataType(DataType.Currency)]
        public double FeeYear { get; set; } // Tax

        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double BrokerageYear { get; set; }

        [Display(Name = "Vinst/förlust")]
        [DataType(DataType.Currency)]
        public double MoneyProfitOrLossYear { get; set; } // ( Årets alla: (säljvärden: aktiar och fonder + utdelningar + ränta) - (köp värderna + skatt + avgifter) )

        [Display(Name = "Vinst/förlust i %")]
        [DataType(DataType.Text)]
        public string? PercentProfitOrLossYear { get; set; } // (Årets alla: (säljvärden: aktiar och fonder + utdelningar + ränta) - (köp värderna + skatt + avgifter) )

        [Display(Name = "Anteckningar")]
        [DataType(DataType.Text)]
        public string? Note { get; set; }
    }
}
