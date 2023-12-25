using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class FrozenFoods
    {
        [Key]
        public int FrozenFoodsId { get; set; }
        public string? Name { get; set; }
        public string? Date { get; set; }
        public string? FreezerCompartment { get; set; } //Frysfack
        public WildMeat WildMeat { get; set; }
        public int Weight { get; set; }
        public string? Notes { get; set; }
    }
}