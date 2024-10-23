using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesDividendViewModel
    {
        public SharesDividendViewModel() => Date = DateTime.Now;

        public int DividendId { get; set; }

        [Required(ErrorMessage = "Skriv in datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Skriv in konto nummer")]
        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "Skriv in typ av transaktion")]
        [Display(Name = "Typ av transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Required(ErrorMessage = "Skriv in ett företagsnamn")]
        [Display(Name = "Värdepapper")]
        [DataType(DataType.Text)]
        public string? Company { get; set; }

        [Required(ErrorMessage = "Skriv hur många aktier")]
        [Display(Name = "Antal")]
        public double NumberOfShares { get; set; }

        [Required(ErrorMessage = "Skriv in pris per aktie")]
        [Display(Name = "Kurs")]
        [DataType(DataType.Text)]
        public string? PricePerShare { get; set; }

        [Required(ErrorMessage = "Skriv in total summa")]
        [Display(Name = "Belopp")]
        [DataType(DataType.Text)]
        public string? TotalAmount { get; set; }

        [Required(ErrorMessage = "Skriv in courtage")]
        [Display(Name = "Courtage")]
        [DataType(DataType.Text)]
        public double Brokerage { get; set; }

        [Required(ErrorMessage = "Skriv in valutan")]
        [Display(Name = "Valuta")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Required(ErrorMessage = "Skriv in företagets ISIN")]
        [Display(Name = "ISIN")]
        [DataType(DataType.Text)]
        public string? ISIN { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}