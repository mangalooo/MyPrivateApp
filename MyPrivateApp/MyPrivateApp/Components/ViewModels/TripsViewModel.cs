using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class TripsViewModel
    {
        [Key]
        public int TripsId { get; set; }

        [Required(ErrorMessage = "Countries_Error_Country")]
        [Display(Name = "Countries_Text_Country")]
        [DataType(DataType.Text)]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Countries_Error_Place")]
        [Display(Name = "Countries_Text_Place")]
        [DataType(DataType.Text)]
        public string? Place { get; set; }

        [Required(ErrorMessage = "Countries_Error_Date")]
        [Display(Name = "Countries_Text_Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Countries_Error_HowLongTravel")]
        [Display(Name = "Countries_Text_HowLongTravel")]
        [DataType(DataType.DateTime)]
        public DateTime HowLongTravel { get; set; }

        [Required(ErrorMessage = "Countries_Error_TravelBuddies")]
        [Display(Name = "Countries_Text_TravelBuddies")]
        [DataType(DataType.MultilineText)]
        public string? TravelBuddies { get; set; }

        [Required(ErrorMessage = "Countries_Error_Description")]
        [Display(Name = "Countries_Text_Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
    }
}