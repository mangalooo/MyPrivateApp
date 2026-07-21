using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesInterestRates
    {
        [Key]
        public int InterestRatesId { get; set; }
        public string? Date { get; set; }
        public string? Account { get; set; }
        public string? TypeOfTransaction { get; set; }
        public double TotalAmount { get; set; }
        public string? Currency { get; set; }
        public bool CalculationFlag { get; set; }
        public string? Note { get; set; }
    }
}