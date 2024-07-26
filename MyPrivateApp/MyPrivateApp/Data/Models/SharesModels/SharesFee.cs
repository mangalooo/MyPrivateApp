using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesFee
    {
        [Key]
        public int SharesFeeId { get; set; }
        public string? Date { get; set; }
        public string? CompanyOrInformation { get; set; }
        public double Tax { get; set; }
        public double Brokerage { get; set; }
        public string? Note { get; set; }
        
        // Error information
        public string? DateOfFee { get; set; }
        public string? Account { get; set; }
        public string? TypeOfTransaction { get; set; }
        public string? ISIN { get; set; }
    }
}