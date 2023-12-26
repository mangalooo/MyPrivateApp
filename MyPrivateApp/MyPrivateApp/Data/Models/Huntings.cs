using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class Huntings
    {
        [Key]
        public int HuntingsId { get; set; }

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
        public string? Place { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Description { get; set; }
    }
}