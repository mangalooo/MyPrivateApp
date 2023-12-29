using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class FrozenFoodViewModel
    {
        [Key]
        public int FrozenFoodId { get; set; }

        [Required(ErrorMessage = "Du måste fylla i frysvarans namn")]
        [Display(Name = "Namn")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Antal ")]
        public int Number { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett datum")]
        [Display( Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Du måste fylla i en frys")]
        [Display(Name = "Frys")]
        public Freezer Freezer { get; set; } //Frys

        [Required(ErrorMessage = "Du måste fylla i ett frysfack")]
        [Display(Name = "Frysfack")]
        public FreezerCompartment FreezerCompartment { get; set; } //Frysfack

        [Required(ErrorMessage = "Du måste välja en typ av frysvara")]
        [Display(Name = "Frysvara")]
        public WildMeat WildMeat { get; set; }

        [Required(ErrorMessage = "Du måste fylla i vikten på frysvaran")]
        [Display(Name = "Vikt (gram)")]
        public int Weight { get; set; }

        [Display(Name = "Anteckning")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }
    }
}