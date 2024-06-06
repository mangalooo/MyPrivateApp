using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class Contacts
    {
        [Key]
        public int ContactsId { get; set; }
        public string? Name { get; set; }
        public string? Birthday { get; set; }
        public string? Address { get; set; }
        public int PostCode { get; set; }
        public string? City { get; set; }
        public string? MarriedPartner { get; set; }
        public string? ChildOne { get; set; }
        public string? ChildTwo { get; set; }
        public string? ChildThree { get; set; }
        public string? ChildFour { get; set; }
        public string? PrivateMail { get; set; }
        public string? WorkEMail { get; set; }
        public string? ExtraMail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? HomePhoneNumber { get; set; }
        public string? WorkPhoneNumber { get; set; }
        public string? ExtraPhoneNumber { get; set; }
        public string? HomePage { get; set; }
        public string? Notes { get; set; }
        public bool ChristmasCard { get; set; }
        public bool Relatives { get; set; }
        public bool Friends { get; set; }
        public bool Colleagues { get; set; }
    }
}