
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels
{
    public class FarmingViewModels
    {
        public FarmingViewModels() => PutSeedDate = DateTime.Now;

        public int FarmingId { get; set; }

        [Display(Name = "Namm")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Typ")]
        [DataType(DataType.Text)]
        public string? Type { get; set; }

        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public string? Place { get; set; }

        [Display(Name = "Frö datum")]
        [DataType(DataType.Date)]
        public DateTime InactiveDate { get; set; }

        [Display(Name = "Frö datum")]
        [DataType(DataType.Date)]
        public DateTime PutSeedDate { get; set; }

        [Display(Name = "Plantera ut datum")]
        [DataType(DataType.Date)]
        public DateTime SetDate { get; set; }

        [Display(Name = "Ta upp datum")]
        [DataType(DataType.Date)]
        public DateTime TakeUpDate { get; set; }

        [Display(Name = "Hur många satta")]
        public int HowMany { get; set; }

        [Display(Name = "Hur många sparade")]
        public int HowManySave { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}
