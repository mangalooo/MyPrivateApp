using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesTaxAndBrokerageViewModel
    {
        [Key]
        public int TaxAndBrokerageId { get; set; }

        [Required(ErrorMessage = "Skriv in ett datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public string? Date { get; set; }

        [Required(ErrorMessage = "Skriv in aktiens skatte avgif")]
        [Display(Name = "Shares_Text_Tax_TaxAndBrokerage")]
        [DataType(DataType.Currency)]
        public int Tax { get; set; }

        [Required(ErrorMessage = "Skriv in aktiens brokerage")] 
        [Display(Name = "Shares_Text_Brokerage")]
        [DataType(DataType.Currency)]
        public int Brokerage { get; set; }
    }
}