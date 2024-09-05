
using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.HuntingViemModels
{
    public class HuntingTowerInspectionViewModels
    {
        [Key]
        public int HuntingTowerInspectionId { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett datum!")]
        [Display(Name = "Senast besiktad")]
        [DataType(DataType.DateTime)]
        public DateTime LastInspected { get; set; }

        [Display(Name = "Besiktad")]
        public bool Inspected { get; set; }

        [Display(Name = "Att göra")]
        public bool InspectedTodo { get; set; }

        [Required(ErrorMessage = "Du måste fylla i vilkan plats!")]
        [Display(Name = "Plats")]
        public HuntingPlaces Place { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett nummer!")]
        [Display(Name = "Nummer")]
        [DataType(DataType.Text)]
        public string? Number { get; set; }

        [Required(ErrorMessage = "Du måste fylla i vad som ska göras!")]
        [Display(Name = "Att göra")]
        public HuntingTodo Todo { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}
