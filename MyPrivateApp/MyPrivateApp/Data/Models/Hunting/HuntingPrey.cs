
using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingPrey
    {
        [Key]
        public int HuntingPreyId { get; set; }

        [DataType(DataType.Text)]
        public string? Date { get; set; }

        [DataType(DataType.Text)]
        public string? Hunter { get; set; }

        public WildAnimal WildAnimal { get; set; }

        [DataType(DataType.Text)]
        public string? Type { get; set; }

        public HuntingForm HuntingForm { get; set; }

        [DataType(DataType.Text)]
        public string? HuntingPass { get; set; }

        [DataType(DataType.Text)]
        public string? Dog { get; set; }

        public HuntingPlaces HuntingPlaces { get; set; }

        [DataType(DataType.Text)]
        public string? Note { get; set; }
    }
}