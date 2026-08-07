using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.FarmWork
{
    public class FarmWorksPlanningCompleted
    {
        [Key]
        public int FarmWorksPlanningCompletedId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public FarmWorkPlaces Place { get; set; }
        public string? Area { get; set; }
        public string? PropertyDesignation { get; set; }
        public FarmWorkPrioritize Prioritize { get; set; }
        public double Hectare { get; set; }
        public double Hours { get; set; }
        public double ThinHours { get; set; } // Gallra tid
        public double ClearingHours { get; set; } //Röjning tid
        public double ForClearingHours { get; set; } // Förröjning tid
        public double ToForwardHours { get; set; } // Skotnings tid
        public string? Notes { get; set; }

        public ICollection<FarmWorks> FarmWorks { get; set; } = new List<FarmWorks>();
    }
}