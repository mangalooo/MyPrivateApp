using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesTaxAndBrokerages
    {
        [Key]
        public int SharesTaxAndBrokerageId { get; set; }
        public string? Date { get; set; }
        public int Tax { get; set; }
        public int Brokerage { get; set; }
    }
}