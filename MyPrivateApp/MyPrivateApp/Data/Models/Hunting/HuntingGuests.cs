
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingGuests
    {
        [Key]
        public int HuntingGuestsId { get; set; }
        public string? Name { get; set; }
        public string? Mail { get; set; }
        public string? MobilePhone { get; set; }
        public bool DogHandler { get; set; }
        public string? Dogs { get; set; }
        public bool PassShooter { get; set; }
        public string? Note { get; set; }
    }
}