using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesPurchasedFunds
    {
        [Key]
        public int SharesPurchasedFundId { get; set; }
        public double Amount { get; set; }
        public double Fee { get; set; }
        public required string? FundName { get; set; } 
        public required string? DateOfPurchase { get; set; }
        public int HowMany { get; set; }
        public string? Note { get; set; }
        public string? TypeOfFund { get; set; }
        public double PricePerFunds { get; set; }
        public string? Currency { get; set; }
        public required string? ISIN { get; set; }
        public string? Account { get; set; }
    }
}