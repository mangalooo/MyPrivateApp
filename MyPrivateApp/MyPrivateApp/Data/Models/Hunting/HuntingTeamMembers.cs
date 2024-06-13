
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingTeamMembers
    {
        [Key]
        public int HuntingTeamMembersId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Name { get; set; }

        [DataType(DataType.Text)]
        public string? Birthday { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Address { get; set; }

        [DataType(DataType.PostalCode)]
        [StringLength(10)]
        public string? PostCode { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? City { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string? Mail { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? MobilePhone { get; set; }

        [DataType(DataType.Text)]
        public string? Note { get; set; }
    }
}
