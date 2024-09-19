
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.Games.ManagerZone
{
    public class ManagerZoneSoldViewModels
    {
        public int ManagerZoneSoldPlayerId { get; set; }

        [Display(Name = "Namn")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Ålder")]
        public int YearsOld { get; set; }

        [Display(Name = "Nummer")]
        public int Number { get; set; }

        [Display(Name = "Köp Datum")]
        [DataType(DataType.Date)]
        public DateTime PurchasedDate { get; set; }

        [Display(Name = "Sälj datum")]
        [DataType(DataType.Date)]
        public DateTime SoldDate { get; set; }

        [Display(Name = "Dagar i klubben")]

        public int DaysInTheClub { get; set; }

        [Display(Name = "Inköpsbelopp")]
        public int PurchaseAmount { get; set; }

        [Display(Name = "Totalt lön")]
        public int SalaryTotal { get; set; }

        [Display(Name = "Total träningsläger kostnad")]
        public int TrainingModeTotalCost { get; set; }

        [Display(Name = "Sälj värdet")]
        public int SoldAmount { get; set; }

        [Display(Name = "Vinst eller förslust")]
        public int MoneyProfitOrLoss { get; set; }

        [Display(Name = "Beskrivning")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}