
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

        [Display(Name = "Att göra")]
        [DataType(DataType.Text)]
        public FarmWorkTodo Todo { get; set; }

        [Display(Name = "Redskap")]
        [DataType(DataType.Text)]
        public FarmWorkTools Tools { get; set; }

        [Display(Name = "Tillbehör")]
        [DataType(DataType.Text)]
        public FarmWorkAccessories Accessories { get; set; }

        [Display(Name = "Fastighetsbeteckning")]
        [DataType(DataType.Text)]
        public string? PropertyDesignation { get; set; }

        [Display(Name = "Område")]
        [DataType(DataType.Text)]
        public string? Area { get; set; }

        [Display(Name = "Timmar")]
        [DataType(DataType.Text)]
        public double Hours { get; set; }
        
        [Display(Name = "Milersättning")]
        [DataType(DataType.Text)]
        public double Milage { get; set; }

        [Display(Name = "Nästa lön")]
        public bool NextSalary { get; set; }

        [Display(Name = "Beskrivning")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }

        [Display(Name = "Skogsplanering")]
        public int? FarmWorksPlanningsId { get; set; }
    }
}