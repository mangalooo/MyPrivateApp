using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.FarmWork
{
    public class FarmWorksPlanning
    {
        [Key]
        public int FarmWorksId { get; set; }
        public string? PlanningDate { get; set; }
        public string? StartDate { get; set; }
        public FarmWorkPlaces Place { get; set; }
        public string? Area { get; set; }
        public Prioritize Prioritize { get; set; }
        public FormWorkTodo FormWorkTodo { get; set; }
        public double Hectare { get; set; }
        public int Hours { get; set; } 
        public string? Notes { get; set; }
    }
}