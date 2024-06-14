
using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingHappenings
    {
        [Key]
        public int SavedHuntingID { get; set; }
        public string? Date { get; set; }
        public string? AboutTheHunt { get; set; }
        public ObservedOrShot ObservedOrShot { get; set; }
        public WildAnimal WildAnimal { get; set; }
        public string? Type { get; set; }
        public string? Number { get; set; }
        public bool DogHandler { get; set; }
        public bool PassShooter { get; set; }
        public string? PassNumber { get; set; }
        public string? Note { get; set; }

        [ForeignKey("HuntingInvitation")]
        public int HuntingInvitation_FK { get; set; }
        public HuntingInvitation? HuntingInvitation { get; set; }

        [ForeignKey("HuntingTeamMembers")]
        public int HuntingTeamMembers_FK { get; set; }
        public HuntingTeamMembers? HuntingTeamMembers { get; set; }

        [ForeignKey("HuntingGuests")]
        public int HuntingGuests_FK { get; set; }
        public HuntingGuests? HuntingGuests { get; set; }
    }
}
