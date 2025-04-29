using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class NewModuleViewModel
    {
        public int NewModuleId { get; set; }

        [Display(Name = "Namn")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Födelsedag")]
        [DataType(DataType.DateTime)]
        public DateTime Birthday { get; set; }

        [Display(Name = "Ålder")]
        [DataType(DataType.Text)]
        public string? Years { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}