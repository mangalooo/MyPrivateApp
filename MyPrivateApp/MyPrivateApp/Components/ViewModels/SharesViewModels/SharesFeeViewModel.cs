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

        [Required(ErrorMessage = "Skriv in aktiens skatteavgif")]
        [Display(Name = "Skatt")]
        [DataType(DataType.Currency)]
        public double Tax { get; set; }

        [Required(ErrorMessage = "Skriv in aktiens courtage")] 
        [Display(Name = "Courtage")]
        [DataType(DataType.Currency)]
        public double Brokerage { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}