using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public class TripsViewModel
    {
        public int TripsId { get; set; }

        [Required(ErrorMessage = "Skriv in ett eller flera länder")]
        [Display(Name = "Land")]
        [DataType(DataType.Text)]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Skriv in ett eller flera städer/platser")]
        [Display(Name = "Stad/ställe")]
        [DataType(DataType.Text)]
        public string? Place { get; set; }

        [Required(ErrorMessage = "Skriv ett datum")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Skriv vilket datum då åkte hem")]
        [Display(Name = "Vilket datum åkte du hem?")]
        [DataType(DataType.DateTime)]
        public DateTime HomeDate { get; set; }

        [Display(Name = "Hur många dagar var resan?")]
        [DataType(DataType.DateTime)]
        public double HowManyDays { get; set; }

        [Required(ErrorMessage = "Vilka resta du med?")]
        [Display(Name = "Vem reste du med?")]
        [DataType(DataType.MultilineText)]
        public string? TravelBuddies { get; set; }

        [Required(ErrorMessage = "Skriv in en resebeskrivning")]
        [Display(Name = "Beskrivning:")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
    }
}