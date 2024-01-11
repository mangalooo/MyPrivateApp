using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesYears
    {
        [Key]
        public int SharesYearsId { get; set; }
        public string? Year { get; set; }
        public double MoneyProfitOrLossYear { get; set; }
        public double PercentProfitOrLossYear { get; set; }
        public double DividendYear { get; set; }
    }
}