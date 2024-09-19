
using System.ComponentModel.DataAnnotations;


namespace MyPrivateApp.Components.ViewModels.Games.ManagerZone
{
    public class ManagerZonePurchasedPlayersViewModels
    {
        public ManagerZonePurchasedPlayersViewModels() => PurchasedDate = DateTime.Now;

        public int ManagerZonePurchasedPlayersId { get; set; }

        [Display(Name = "Namn")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Ålder")]
        public int YearsOld { get; set; }

        [Display(Name = "Nummer")]
        public int Number { get; set; }

        [Display(Name = "Inköpsdatum")]
        [DataType(DataType.Date)]
        public DateTime PurchasedDate { get; set; }

        [Display(Name = "Inköpsbelopp")]
        public int PurchaseAmount { get; set; }

        [Display(Name = "Dagar i klubben")]
        public int DaysInTheClub { get; set; }

        [Display(Name = "Lön")]
        public int Salary { get; set; }

        [Display(Name = "Total utbetalad lön")]
        public int SalaryTotal { get; set; }

        [Display(Name = "Ändra lön")]
        public int SalaryChange { get; set; }

        [Display(Name = "Spara gammal lön")]
        public int SalarySaved { get; set; }

        [Display(Name = "Denna träningsläger kostnad")]
        public int TrainingModeCost { get; set; }

        [Display(Name = "Total träningsläger kostnad")]
        public int TrainingModeTotalCost { get; set; }

        [Display(Name = "Total kostnad just nu")]
        public int TotalCostRightNow { get; set; }

        [Display(Name = "Sälj datum")]
        [DataType(DataType.Date)]
        public DateTime SoldDate { get; set; }

        [Display(Name = "Sälj värdet")]
        public int SoldAmount { get; set; }

        [Display(Name = "Beskrivning")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}