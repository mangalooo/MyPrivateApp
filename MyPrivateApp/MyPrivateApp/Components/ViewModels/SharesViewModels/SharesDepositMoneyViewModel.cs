using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesDepositMoneyViewModel
    {
        public int DepositMoneyId { get; set; }

        [Required(ErrorMessage = "Skriv in ett datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public string? DateOfDepositMoney { get; set; }

        [Required(ErrorMessage = "Skriv in beloppet från/till dit bankkonto")]
        [Display(Name = "Banköverföringar")]
        [DataType(DataType.Currency)]
        public int MyDepositMoney { get; set; }

        [Display(Name = "In- och utbetalningar")]
        [DataType(DataType.Text)]
        public SubmitOrWithdraw SubmitOrWithdraw { get; set; } // In- och utbetalningar till och från mitt bankkonto. Check på "Välj"
    }
}