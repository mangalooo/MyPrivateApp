using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesFeeViewModel
    {
        public SharesFeeViewModel() => Date = DateTime.Now;

        public int SharesFeeId { get; set; }

        [Required(ErrorMessage = "Skriv in ett datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Namn")]
        [DataType(DataType.Text)]
        public string? CompanyOrInformation { get; set; }

        [Display(Name = "Skatt")]
        [DataType(DataType.Currency)]
        public double Tax { get; set; }

        [Display(Name = "Avgift")]
        [DataType(DataType.Currency)]
        public double Fee { get; set; }

        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double Brokerage { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        // Error information
        public DateTime DateOfFee { get; set; }
        public string? Account { get; set; }
        public string? TypeOfTransaction { get; set; }
        public string? ISIN { get; set; }
    }
}