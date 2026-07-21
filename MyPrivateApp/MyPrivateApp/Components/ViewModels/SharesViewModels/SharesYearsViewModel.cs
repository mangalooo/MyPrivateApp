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
        [DataType(DataType.Text)]
        public string? MoneyProfitOrLossYear { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Vinst/förlust i %")]
        [DataType(DataType.Text)]
        public string? PercentProfitOrLossYear { get; set; }
    }
}