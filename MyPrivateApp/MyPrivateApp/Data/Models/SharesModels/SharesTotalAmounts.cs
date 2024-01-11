using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesTotalAmounts
    {
        [Key]
        public int TotalAmountId { get; set; }
        public double TotalAmount { get; set; }
    }
}