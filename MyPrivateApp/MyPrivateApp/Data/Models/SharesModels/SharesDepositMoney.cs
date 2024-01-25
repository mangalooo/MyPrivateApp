using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesDepositMoney
    {
        [Key]
        public int DepositMoneyId { get; set; }
        public string? Date { get; set; }
        public double DepositMoney { get; set; }
        public SubmitOrWithdraw SubmitOrWithdraw { get; set; } // In och utbetalningar till och från mitt bankkonto
        public string? TypeOfTransaction { get; set; }
        public string? TransferOptions { get; set; } // Text information om hur pengar kommer in och ut (Avanza) 
        public string? Account { get; set; }
        public string? Currency { get; set; }
        public string? Note { get; set; }
    }
}