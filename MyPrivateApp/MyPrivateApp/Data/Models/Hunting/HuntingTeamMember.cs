
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingTeamMember
    {
        [Key]
        public int MemberId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? LastName { get; set; }

        [DataType(DataType.Text)]
        public string? Birthday { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Address { get; set; }

        public string? StreetNumber { get; set; }

        [DataType(DataType.PostalCode)]
        [StringLength(10)]
        public string? PostCode { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? City { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string? Mail { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? Phone { get; set; }

        [DataType(DataType.Text)]
        public string? Note { get; set; }
    }
}
