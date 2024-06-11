
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

        [DataType(DataType.Text)]
        [StringLength(50)]
        public WildAnimal WildAnimal { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Type { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Dog { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public HuntingPlaces HuntingPlaces { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Note { get; set; }
    }
}