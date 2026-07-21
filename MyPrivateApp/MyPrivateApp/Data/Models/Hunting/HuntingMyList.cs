
using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingMyList
    {
        [Key]
        public int HuntingMyListId { get; set; }

        [DataType(DataType.Text)]
        public string? Date { get; set; }

        public WildAnimal WildAnimal { get; set; }

        public HuntingForm HuntingForm { get; set; }

        [DataType(DataType.Text)]
        public string? Type { get; set; }

        [DataType(DataType.Text)]
        public string? Dog { get; set; }

        public HuntingPlaces HuntingPlaces { get; set; }

        [DataType(DataType.Text)]
        public string? Note { get; set; }
    }
}