using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.FarmWork
{
    public class FarmWorksPlanningViewModels
    {
        public FarmWorksPlanningViewModels() => Date = DateTime.Now;

        public int FarmWorksId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Text)]
        public DateTime Date { get; set; }

        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public FarmWorkPlaces Place { get; set; }

        [Display(Name = "Område")]
        public string? Area { get; set; }

        [Display(Name = "Att göra")]
        [DataType(DataType.MultilineText)]
        public FormWorkTodo FormWorkTodo { get; set; }

        [Display(Name = "Prioritera")]
        [DataType(DataType.Text)]
        public Prioritize Prioritize { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }
    }
}