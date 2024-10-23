using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesDepositMoneyViewModel
    {
        public SharesDepositMoneyViewModel() => Date = DateTime.Now;

        public int DepositMoneyId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Belopp")]
        [DataType(DataType.Text)]
        public string? DepositMoney { get; set; }

        [Display(Name = "In- eller utbetalningar")]
        [DataType(DataType.Text)]
        public SubmitOrWithdraw SubmitOrWithdraw { get; set; } // In- och utbetalningar till och från mitt bankkonto. Check på "Välj"

        [Display(Name = "typ av transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Display(Name = "Vart pengarna kommer ifrån")]
        [DataType(DataType.Text)]
        public string? TransferOptions { get; set; } // Text information om hur pengar kommer in och ut (Avanza) 

        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? Account { get; set; }

        [Display(Name = "Valuta")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Display(Name = "Antecklingar")]
        [DataType(DataType.Text)]
        public string? Note { get; set; }
    }
}