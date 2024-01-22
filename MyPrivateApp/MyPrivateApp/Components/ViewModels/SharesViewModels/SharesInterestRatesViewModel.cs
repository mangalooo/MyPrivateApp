using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesInterestRatesViewModel
    {
        [Key]
        public int InterestRatesId { get; set; }

        [Required(ErrorMessage = "Skriv in datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? Account { get; set; }

        [Required(ErrorMessage = "Skriv in typ av transaktion")]
        [Display(Name = "Transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Required(ErrorMessage = "Skriv in total summa")]
        [Display(Name = "Belopp")]
        [DataType(DataType.Text)]
        public double TotalAmount { get; set; }

        [Required(ErrorMessage = "Skriv in valutan")]
        [Display(Name = "Valuta")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}