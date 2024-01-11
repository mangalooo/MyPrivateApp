using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesSolds
    {
        [Key]
        public int SharesSoldId { get; set; }
        public double Amount { get; set; } // Totalbelopp från köpta värdet
        public double AmountSold { get; set; } // Totalbelopp från solda värdet 
        public int Brokerage { get; set; } //Courtage, köp kostnad
        public string? CompanyName { get; set; }
        public string? DateOfPurchase { get; set; } //  Köpsdatum. String? 
        public string? DateOfSold { get; set; } //  Säljsdatum. String?
        public int HowMany { get; set; }
        public string? Note { get; set; }
        public string? TypeOfShares { get; set; }
        public double PricePerShares { get; set; } // Uträkning nytt värde
        public double PricePerSharesSold { get; set; } // Uträkning nytt värde

        public double MoneyProfitOrLoss { get; set; } //Vinst eller förlust i pengar
        public string? PercentProfitOrLoss { get; set; } //Vinst eller förlust i %
        public string? Currency { get; set; }
        public string? ISIN { get; set; }
        public string? Account { get; set; }
    }
}