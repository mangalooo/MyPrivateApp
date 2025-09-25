
using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.FarmWork
{
    public class FarmWorksViewModels
    {
        public FarmWorksViewModels() => Date = DateTime.Now;

        public int FarmWorksId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Text)]
        public DateTime Date { get; set; }

        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public FarmWorkPlaces Place { get; set; }

        [Display(Name = "Fastighetsbeteckning")]
        [DataType(DataType.Text)]
        public string? PropertyDesignation { get; set; }

        [Display(Name = "Område")]
        [DataType(DataType.Text)]
        public string? Area { get; set; }

        [Display(Name = "Timmar")]
        [DataType(DataType.Text)]
        public double Hours { get; set; }

        [Display(Name = "Nästa lön")]
        public bool NextSalary { get; set; }

        [Display(Name = "Beskrivning")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}