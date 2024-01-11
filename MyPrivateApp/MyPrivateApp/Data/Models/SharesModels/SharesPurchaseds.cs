using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesPurchaseds //Spara köpta aktier
    {
        [Key]
        public int SharesPurchasedId { get; set; }
        public double Amount { get; set; } // totalbelopp. De ska även funka att köpa till fler aktier på denna post
        public int Brokerage { get; set; } //courtage, köp kostnad. De ska även funka att köpa till fler aktier på denna post
        public string? CompanyName { get; set; } 
        public string? DateOfPurchase { get; set; } //  Köp datum
        public int HowMany { get; set; } // De ska även funka att köpa till fler aktier på denna post
        public string? Note { get; set; }
        public string? TypeOfShares { get; set; }
        public double PricePerShares { get; set; } // De ska även funka att köpa till fler aktier på denna post
        public double Dividend { get; set; } //Utdelning // De ska även funka att köpa till fler utdelningar på denna post
        public string? Currency { get; set; }
        public string? ISIN { get; set; }
        public string? Account { get; set; }
    }
}