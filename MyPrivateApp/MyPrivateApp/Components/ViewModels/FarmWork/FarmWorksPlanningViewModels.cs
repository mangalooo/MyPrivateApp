using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.FarmWork
{
    public class FarmWorksPlanningViewModels
    {
        public FarmWorksPlanningViewModels() => PlanningDate = DateTime.Now;

        public int FarmWorksPlanningsId { get; set; }

        [Display(Name = "Planeringsdatum")]
        [DataType(DataType.Text)]
        public DateTime PlanningDate { get; set; }

        [Display(Name = "Startdatum")]
        [DataType(DataType.Text)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public FarmWorkPlaces Place { get; set; }

        [Display(Name = "Område")]
        [DataType(DataType.Text)]
        public string? Area { get; set; }

        [Display(Name = "Fastighetsbeteckning")]
        public string? PropertyDesignation { get; set; }

        [Display(Name = "Prioritera")]
        [DataType(DataType.Text)]
        public FarmWorkPrioritize Prioritize { get; set; }

        [Display(Name = "Hektar")]
        public double Hectare { get; set; }

        [Display(Name = "Timmar")]
        public double Hours { get; set; }

        [Display(Name = "Gallra")]
        public double ThinHours { get; set; }

        [Display(Name = "Röjning")]
        public double ClearingHours { get; set; }

        [Display(Name = "Förröjning")]
        public double ForClearingHours { get; set; }

        [Display(Name = "Skotning")]
        public double ToForwardHours { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        //FarmWorksPlanningCompleted

        [Display(Name = "Slutdatum")]
        [DataType(DataType.Text)]
        public DateTime EndDate { get; set; }
    }
}