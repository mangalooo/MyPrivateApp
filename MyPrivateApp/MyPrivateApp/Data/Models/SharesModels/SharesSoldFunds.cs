using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesSoldFunds 
    { 
        [Key]
        public int SharesSoldFundId { get; set; }
        public double Amount { get; set; }
        public double AmountSold { get; set; }
        public double Fee { get; set; }
        public required string? FundName { get; set; }
        public required string? DateOfPurchase { get; set; } 
        public required string? DateOfSold { get; set; }
        public double HowMany { get; set; }
        public string? Note { get; set; }
        public string? TypeOfFund{ get; set; }
        public double PricePerFunds { get; set; }
        public double PricePerFundsSold { get; set; }
        public double MoneyProfitOrLoss { get; set; }
        public string? PercentProfitOrLoss { get; set; }
        public string? Currency { get; set; }
        public required string? ISIN { get; set; }
        public string? Account { get; set; }
    }
}