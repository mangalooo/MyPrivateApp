using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesDividendViewModel
    {
        public SharesDividendViewModel() => Date = DateTime.Now;

        public int DividendId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? AccountNumber { get; set; }

        [Display(Name = "Typ av transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Display(Name = "Värdepapper")]
        [DataType(DataType.Text)]
        public string? Company { get; set; }

        [Display(Name = "Antal")]
        public double NumberOfShares { get; set; }

        [Display(Name = "Kurs")]
        [DataType(DataType.Text)]
        public string? PricePerShare { get; set; }

        [Display(Name = "Belopp")]
        [DataType(DataType.Text)]
        public string? TotalAmount { get; set; }

        [Display(Name = "Valuta")]
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