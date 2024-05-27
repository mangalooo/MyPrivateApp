using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class ShopingListViewModels
    {
        [Key]
        public int ShopingListId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Du måste skriva var djuret sköts")]
        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public string? Place { get; set; }

        [Display(Name = "Inköpslistan")]
        [DataType(DataType.MultilineText)]
        public string? List { get; set; }
    }
}