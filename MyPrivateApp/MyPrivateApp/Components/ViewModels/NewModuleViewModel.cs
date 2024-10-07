using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class NewModuleViewModel
    {
        public int NewModuleId { get; set; }

        [Required(ErrorMessage = "Skriv in ett namn")]
        [Display(Name = "Namn")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Skriv in en fördelsedag")]
        [Display(Name = "Födelsedag")]
        [DataType(DataType.DateTime)]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Skriv in en ålder")]
        [Display(Name = "Ålder")]
        [DataType(DataType.Text)]
        public string? Years { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}