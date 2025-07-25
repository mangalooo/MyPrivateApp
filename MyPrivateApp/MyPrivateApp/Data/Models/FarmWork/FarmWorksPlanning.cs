using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.FarmWork
{
    public class FarmWorksPlanning
    {
        [Key]
        public int FarmWorksPlanningsId { get; set; }
        public string? PlanningDate { get; set; }
        public string? StartDate { get; set; }
        public FarmWorkPlaces Place { get; set; }
        public string? Area { get; set; }
        public FarmWorkPrioritize Prioritize { get; set; }
        public FarmWorkTodo Todo { get; set; }
        public double Hectare { get; set; }
        public double Hours { get; set; } 
        public string? Notes { get; set; }
    }
}