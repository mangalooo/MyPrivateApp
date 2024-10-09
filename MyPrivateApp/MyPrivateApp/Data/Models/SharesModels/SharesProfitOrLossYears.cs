using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesProfitOrLossYears
    {
        [Key]
        public int SharesProfitOrLossYearsId { get; set; }
        public string? Year { get; set; }
        public double SharesYear { get; set; }
        public double FundsYear { get; set; }
        public double DividendYear { get; set; }
        public double InterestRatesYear { get; set; }
        public double FeeYear { get; set; } // Tax
        public double BrokerageYear { get; set; }
        public double MoneyProfitOrLossYear { get; set; } // ( Årets alla: (säljvärden: aktiar och fonder + utdelningar + ränta) - (köp värderna + skatt + avgifter) )
        public string? PercentProfitOrLossYear { get; set; } // ( Årets alla: (säljvärden + utdelningar) - (köp värderna + skatt + avgifter) )
        public string? Note { get; set; }
    }
}