using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesSolds
    {
        [Key]
        public int SharesSoldId { get; set; }
        public double Amount { get; set; }
        public double AmountSold { get; set; }
        public double Brokerage { get; set; }
        public required string CompanyName { get; set; }
        public required string DateOfPurchase { get; set; }
        public required string DateOfSold { get; set; }
        public int HowMany { get; set; }
        public string? Note { get; set; }
        public string? TypeOfShares { get; set; }
        public double PricePerShares { get; set; }
        public double PricePerSharesSold { get; set; }
        public double MoneyProfitOrLoss { get; set; }
        public string? PercentProfitOrLoss { get; set; }
        public string? Currency { get; set; }
        public required string ISIN { get; set; }
        public bool CalculationFlag { get; set; }
        public string? Account { get; set; }
    }
}