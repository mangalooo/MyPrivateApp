using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.FarmWork
{
    public class FarmWorksPlanning
    {
        [Key]
        public int FarmWorksId { get; set; }
        public string? Date { get; set; }
        public FarmWorkPlaces Place { get; set; }
        public double Hours { get; set; }
        public bool NextSalary { get; set; }
        public string? Note { get; set; }
    }
}