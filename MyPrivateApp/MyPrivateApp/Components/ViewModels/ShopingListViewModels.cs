
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels
{
    public class ShopingListViewModels
    {
        public ShopingListViewModels() => Date = DateTime.Now;

        public int ShopingListId { get; set; }

        [Display(Name = "Name")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public string? Place { get; set; }

        [Display(Name = "Inköpslistan")]
        [DataType(DataType.MultilineText)]
        public string? List { get; set; }
    }
}