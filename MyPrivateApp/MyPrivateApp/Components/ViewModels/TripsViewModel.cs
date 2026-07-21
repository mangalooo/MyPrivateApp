using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class TripsViewModel
    {
        public int TripsId { get; set; }

        [Display(Name = "Land")]
        [DataType(DataType.Text)]
        public string? Country { get; set; }

        [Display(Name = "Stad/ställe")]
        [DataType(DataType.Text)]
        public string? Place { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Vilket datum åkte du hem?")]
        [DataType(DataType.DateTime)]
        public DateTime HomeDate { get; set; }

        [DataType(DataType.DateTime)]
        public double HowManyDays { get; set; }

        [Display(Name = "Vem reste du med?")]
        [DataType(DataType.MultilineText)]
        public string? TravelBuddies { get; set; }

        [Display(Name = "Beskrivning:")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
    }
}