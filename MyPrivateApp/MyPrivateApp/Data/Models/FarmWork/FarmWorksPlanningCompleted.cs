using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.FarmWork
{
    public class FarmWorksPlanningCompleted
    {
        [Key]
        public int FarmWorksId { get; set; }
        public string? PlanningDate { get; set; }
        public string? StartDate { get; set; }
        public FarmWorkPlaces Place { get; set; }
        public string? Area { get; set; }
        public FarmWorkPrioritize Prioritize { get; set; }
        public FarmWorkTodo Todo { get; set; }
        public double Hectare { get; set; }
        public int Hours { get; set; } 
        public string? Notes { get; set; }
        public string? EndDate { get; set; }
    }
}