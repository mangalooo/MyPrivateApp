using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesImportsFileViewModel
    {
        public SharesImportsFileViewModel() => Date = DateTime.Now;

        public int SharesImportsFileId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Filnamn")]
        [DataType(DataType.Text)]
        public string? FileName { get; set; }

        [Display(Name = "Antal av transaktion")]
        [DataType(DataType.Text)]
        public int NumbersOfTransaction { get; set; }

        [Display(Name = "Felmeddelanden")]
        [DataType(DataType.MultilineText)]
        public string? Errors { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}