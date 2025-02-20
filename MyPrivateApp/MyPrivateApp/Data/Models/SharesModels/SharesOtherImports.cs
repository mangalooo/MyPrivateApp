using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesOtherImports
    {
        [Key]
        public int OtherImportsId { get; set; }
        public string? Date { get; set; }
        public string? Account { get; set; }
        public string? TypeOfTransaction { get; set; }
        public string? Company { get; set; }
        public double NumberOfShares { get; set; }
        public double PricePerShare { get; set; }
        public double Amount { get; set; }
        public double Brokerage { get; set; }
        public string? Currency { get; set; }
        public string? ISIN { get; set; }
        public string? Note { get; set; }
    }
}