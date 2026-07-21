using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class NewModules
    {
        [Key]
        public int NewModuleId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Birthday { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public int Years { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string? Note { get; set; }
    }
}