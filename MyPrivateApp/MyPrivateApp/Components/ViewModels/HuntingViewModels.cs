using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class HuntingViewModels
    {
        [Key]
        public int HuntingsId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Du måste skriva vilket vilt")]
        [Display(Name = "Vilt")]
        public WildAnimal WildAnimal { get; set; }

        [Required(ErrorMessage = "Du måste skriva vilken typ av vilt")]
        [Display(Name = "Typ")]
        [DataType(DataType.Text)]
        public string? Type { get; set; }

        [Display(Name = "Hund")]
        [DataType(DataType.Text)]
        public string? Dog { get; set; }

        [Required(ErrorMessage = "Du måste skriva var djuret sköts")]
        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public string? Place { get; set; }

        [Display(Name = "Beskrivning")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
    }
}