using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesInterestRatesViewModel
    {
        public SharesInterestRatesViewModel() => Date = DateTime.Now;

        public int InterestRatesId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? Account { get; set; }

        [Display(Name = "Transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Display(Name = "Belopp")]
        [DataType(DataType.Text)]
        public double TotalAmount { get; set; }

        [Display(Name = "Valuta")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}