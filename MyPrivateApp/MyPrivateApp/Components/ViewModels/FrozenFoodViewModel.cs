using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class FrozenFoodViewModel
    {
        public int FrozenFoodId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Namn")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Typ")]
        [DataType(DataType.Text)]
        public string? Type { get; set; }

        [Display(Name = "Antal ")]
        public int Number { get; set; }

        [Display(Name = "Frys plats")]
        public FreezerPlaces Place { get; set; }

        [Display(Name = "Frysfack")]
        public FreezerCompartment FreezerCompartment { get; set; }

        [Display(Name = "Frysvara")]
        public FreezerFrozenGoods FrozenGoods { get; set; } 

        [Display(Name = "Vikt (gram)")]
        public double Weight { get; set; }

        [Display(Name = "Anteckning")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }
    }
}