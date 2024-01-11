using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesYearsViewModel
    {
        public int SharesYearsId { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "År")]
        [DataType(DataType.Date)]
        public string? Year { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Vinst/förlust")]
        [DataType(DataType.Date)]
        public double MoneyProfitOrLossYear { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Vinst/förlust i %")]
        [DataType(DataType.Date)]
        public double PercentProfitOrLossYear { get; set; }
    }
}