using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class FrozenFoods
    {
        [Key]
        public int FrozenFoodsId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Date { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Name { get; set; }

        public int Number { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public Freezer Freezer { get; set; } //Frys

        [DataType(DataType.Text)]
        [StringLength(50)]
        public FreezerCompartment FreezerCompartment { get; set; } //Frysfack 

        [DataType(DataType.Text)]
        [StringLength(50)]
        public WildMeat WildMeat { get; set; }

        public int Weight { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}