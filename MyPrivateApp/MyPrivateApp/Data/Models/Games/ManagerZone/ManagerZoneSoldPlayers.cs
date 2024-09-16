
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Games.ManagerZone
{
    public class ManagerZoneSoldPlayers
    {
        [Key]
        public int ManagerZoneSoldPlayerId { get; set; }
        public string? PurchasedDate { get; set; }
        public string? SoldDate { get; set; }
        public int PurchaseAmount { get; set; }
        public int SalaryTotal { get; set; }
        public int DaysInTheClub { get; set; }
        public int TrainingModeTotalCost { get; set; }
        public int SoldAmount { get; set; }
        public int MoneyProfitOrLoss { get; set; }
        public string? Note { get; set; }
    }
}
