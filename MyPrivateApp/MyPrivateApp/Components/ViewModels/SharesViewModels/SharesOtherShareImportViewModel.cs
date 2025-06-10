using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesOtherShareImportViewModel
    {
        public SharesOtherShareImportViewModel() => Date = DateTime.Now;

        public int OtherImportsId { get; set; }

        [Display(Name = "datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? Account { get; set; }

        [Display(Name = "typ av transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? Company { get; set; }

        [Display(Name = "Antal")]
        [DataType(DataType.Text)]
        public double NumberOfShares { get; set; }

        [Display(Name = "Kurs")]
        [DataType(DataType.Text)]
        public double PricePerShare { get; set; }

        [Display(Name = "Belopp")]
        [DataType(DataType.Text)]
        public double Amount { get; set; }

        [Display(Name = "Courtage")]
        [DataType(DataType.Text)]
        public double Brokerage { get; set; }

        [Display(Name = "Valutan")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Display(Name = "ISIN")]
        [DataType(DataType.Text)]
        public string? ISIN { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}