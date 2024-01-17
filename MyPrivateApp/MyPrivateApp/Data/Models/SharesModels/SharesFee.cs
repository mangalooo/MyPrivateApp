using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesFee
    {
        [Key]
        public int SharesFeeId { get; set; }
        public string? Date { get; set; }
        public double Tax { get; set; }
        public double Brokerage { get; set; }
        public string? Note { get; set; }
    }
}