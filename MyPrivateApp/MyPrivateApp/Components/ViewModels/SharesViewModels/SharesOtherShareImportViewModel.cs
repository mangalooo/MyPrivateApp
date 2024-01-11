using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesOtherShareImportViewModel
    {
        [Key]
        public int OtherImportsId { get; set; }

        [Required(ErrorMessage = "Skriv in datum")]
        [Display(Name = "datum")]
        [DataType(DataType.Date)]
        public string? Date { get; set; }

        [Required(ErrorMessage = "Skriv in konto nummer")]
        [Display(Name = "Konto")]
        [DataType(DataType.Text)]
        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "Skriv in typ av transaktion")]
        [Display(Name = "Transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Required(ErrorMessage = "Skriv in ett företag")]
        [Display(Name = "Företag")]
        [DataType(DataType.Text)]
        public string? Company { get; set; }

        [Required(ErrorMessage = "Skriv hur många aktier")]
        [Display(Name = "Antal")]
        [DataType(DataType.Text)]
        public string? NumberOfSharesString { get; set; }

        [Required(ErrorMessage = "Skriv in pris per aktie")]
        [Display(Name = "Kurs")]
        [DataType(DataType.Text)]
        public string? PricePerShareString { get; set; }

        [Required(ErrorMessage = "Skriv in total summa")]
        [Display(Name = "Belopp")]
        [DataType(DataType.Text)]
        public string? TotalAmountString { get; set; }

        [Required(ErrorMessage = "Skriv in courtage")]
        [Display(Name = "Courtage")]
        [DataType(DataType.Text)]
        public string? Brokerage { get; set; }

        [Required(ErrorMessage = "Skriv in valutan")]
        [Display(Name = "Valutan")]
        [DataType(DataType.Text)]
        public string? Currency { get; set; }

        [Required(ErrorMessage = "Skriv in företagets ISIN")]
        [Display(Name = "ISIN")]
        [DataType(DataType.Text)]
        public string? ISIN { get; set; }
    }
}