using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class Trips
    {
        [Key]
        public int TripsId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Country { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Place { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Date { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? HomeDate { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public double HowManyDays { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string? TravelBuddies { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string? Description { get; set; }
    }
}