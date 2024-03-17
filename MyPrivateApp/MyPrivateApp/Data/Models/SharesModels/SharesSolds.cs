using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesSolds
    {
        [Key]
        public int SharesSoldId { get; set; }
        public double Amount { get; set; } // Totalbelopp från köpta värdet
        public double AmountSold { get; set; } // Totalbelopp från solda värdet 
        public double Brokerage { get; set; } //Courtage, köp kostnad
        public required string CompanyName { get; set; }
        public required string DateOfPurchase { get; set; } //  Köpsdatum. String? 
        public required string DateOfSold { get; set; } //  Säljsdatum. String?
        public int HowMany { get; set; }
        public string? Note { get; set; }
        public string? TypeOfShares { get; set; }
        public double PricePerShares { get; set; } // Uträkning nytt värde
        public double PricePerSharesSold { get; set; } // Uträkning nytt värde
        public double MoneyProfitOrLoss { get; set; } //Vinst eller förlust i pengar
        public string? PercentProfitOrLoss { get; set; } //Vinst eller förlust i %
        public string? Currency { get; set; }
        public required string ISIN { get; set; }
        public string? Account { get; set; }
    }
}