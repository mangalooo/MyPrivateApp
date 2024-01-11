using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesDividends
    {
        [Key]
        public int DividendId { get; set; }
        public string? Date { get; set; }
        public string? AccountNumber { get; set; }
        public string? TypeOfTransaction { get; set; }
        public string? Company { get; set; }
        public string? NumberOfSharesString { get; set; }
        public string? PricePerShareString { get; set; }
        public string? TotalAmountString { get; set; }
        public string? Currency { get; set; }
        public string? ISIN { get; set; }
    }
}
