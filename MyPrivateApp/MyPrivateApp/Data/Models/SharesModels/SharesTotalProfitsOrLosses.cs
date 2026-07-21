using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesTotalProfitsOrLosses
    {
        [Key]
        public int SharesTotalProfitOrLossId { get; set; }
        public double TotalProfitOrLoss { get; set; }
    }
}