using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.FarmWork
{
    public class FarmWorks
    {
        [Key]
        public int FarmWorksId { get; set; }

        public string? Date { get; set; }
        public FarmWorkPlaces Place { get; set; }
        public FarmWorkTodo Todo { get; set; }
        public FarmWorkTools Tools { get; set; }
        public FarmWorkAccessories Accessories { get; set; }
        public string? PropertyDesignation { get; set; }
        public string? Area { get; set; }
        public double Hours { get; set; }
        public double Milage { get; set; }
        public bool NextSalary { get; set; }
        public string? Note { get; set; }

        public int? FarmWorksPlanningsId { get; set; }
        public FarmWorksPlanning? FarmWorksPlanning { get; set; }
    }
}
