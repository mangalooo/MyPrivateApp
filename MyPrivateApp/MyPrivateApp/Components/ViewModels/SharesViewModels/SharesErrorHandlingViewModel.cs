using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesErrorHandlingViewModel
    {
        public int ErrorHandlingsId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public string? Date { get; set; }

        [Display(Name = "Felmeddelande")]
        [DataType(DataType.MultilineText)]
        public string? ErrorMessage { get; set; }
    }
}