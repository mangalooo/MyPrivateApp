
using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingInvitation
    {
        [Key]
        public int HuntInvitationId { get; set; }
        public string? InvitationName { get; set; }
        public HuntingName HuntingName { get; set; }
        public HuntingForm HuntingForm { get; set; }
        public string? Date { get; set; }
        public string? LastDateToReport { get; set; }
        public string? Time { get; set; }
        public HuntingPlaces Place { get; set; }
        public string? AboutTheHunt { get; set; }
        public List<HuntingTeamMembers>? Members { get; set; }
        public List<HuntingGuests>? Guests { get; set; }
        public string? Note { get; set; }
    }
}
