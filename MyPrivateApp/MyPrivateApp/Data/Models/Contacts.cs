using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class Contacts
    {
        [Key]
        public int ContactsId { get; set; }

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
        public int PostCode { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? City { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string? PrivateMail { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string? WorkEMail { get; set; }

        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string? ExtraMail { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? HomePhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? WorkPhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? ExtraPhoneNumber { get; set; }

        [DataType(DataType.Url)]
        public string? HomePage { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string? Notes { get; set; }

        public bool ChristmasCard { get; set; }

        public bool Relatives { get; set; }

        public bool Friends { get; set; }

        public bool Colleagues { get; set; }
    }
}