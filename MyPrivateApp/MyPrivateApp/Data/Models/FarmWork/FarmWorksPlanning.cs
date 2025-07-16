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
        public string? Area { get; set; }
        public Prioritize Prioritize { get; set; }
        public FormWorkTodo FormWorkTodo { get; set; }
        public string? Notes { get; set; }
    }
}