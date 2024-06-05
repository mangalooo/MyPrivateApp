using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Farming
{
    public class FarmingsInactive
    {
        [Key]
        public int FarmingId { get; set; }
        public string? InactiveDate { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Place { get; set; }
        public string? PutSeedDate { get; set; }
        public string? SetDate { get; set; }
        public string? TakeUpDate { get; set; }
        public int HowMany { get; set; }
        public int HowManySave { get; set; }
        public string? Note { get; set; }
    }
}
