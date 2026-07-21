
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingTeamMembers
    {
        [Key]
        public int HuntingTeamMembersId { get; set; }
        public string? Name { get; set; }
        public string? Birthday { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? Mail { get; set; }
        public string? MobilePhone { get; set; }
        public string? Note { get; set; }
    }
}
