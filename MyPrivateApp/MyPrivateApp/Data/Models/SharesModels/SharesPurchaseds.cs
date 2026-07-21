using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesPurchaseds //Spara köpta aktier
    {
        [Key]
        public int SharesPurchasedId { get; set; }
        public double Amount { get; set; } // totalbelopp. De ska även funka att köpa till fler aktier på denna post
        public double Brokerage { get; set; } //courtage, köp kostnad. De ska även funka att köpa till fler aktier på denna post
        public required string CompanyName { get; set; } 
        public required string DateOfPurchase { get; set; } //  Köp datum
        public double HowMany { get; set; } // De ska även funka att köpa till fler aktier på denna post
        public string? Note { get; set; }
        public string? TypeOfShares { get; set; }
        public double PricePerShares { get; set; } // De ska även funka att köpa till fler aktier på denna post
        public string? Currency { get; set; }
        public required string ISIN { get; set; }
        public string? Account { get; set; }
    }
}