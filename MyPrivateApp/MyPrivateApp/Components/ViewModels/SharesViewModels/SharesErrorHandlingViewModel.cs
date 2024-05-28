using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesErrorHandlingViewModel
    {
        public SharesErrorHandlingViewModel() => Date = DateTime.Now;

        public int ErrorHandlingsId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Felmeddelande")]
        [DataType(DataType.MultilineText)]
        public string? ErrorMessage { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}