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
        public string? Area { get; set; }

        [Display(Name = "Att göra")]
        [DataType(DataType.MultilineText)]
        public FarmWorkTodo Todo { get; set; }

        [Display(Name = "Prioritera")]
        [DataType(DataType.Text)]
        public FarmWorkPrioritize Prioritize { get; set; }

        public double Hectare { get; set; }

        [Display(Name = "Timmar")]
        public int Hours { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        //FarmWorksPlanningCompleted

        [Display(Name = "Slutdatum")]
        [DataType(DataType.Text)]
        public DateTime EndDate { get; set; }
    }
}