
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Games.ManagerZone
{
    public class ManagerZonePurchasedPlayers
    {
        [Key]
        public int ManagerZonePurchasedPlayersId { get; set; }
        public string? PurchasedDate { get; set; }
        public int PurchaseAmount { get; set; }
        public int Salary { get; set; }
        public int SalarySaved { get; set; } // If the salary change, save the old total salary here.
        public int TrainingModeTotalCost { get; set; }
        public string? Note { get; set; }
    }
}
