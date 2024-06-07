
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public sealed class ContactsViewModels
    {
        [Key]
        public int ContactsId { get; set; }

        [Display(Name = "Namn ")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Display(Name = "Födelsedag ")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Display(Name = "Adress ")]
        [DataType(DataType.Text)]
        public string? Address { get; set; }

        [Display(Name = "Postnummer ")]
        [DataType(DataType.PostalCode)]
        public int PostCode { get; set; }

        [Display(Name = "Stad ")]
        [DataType(DataType.Text)]
        public string? City { get; set; }

        [Display(Name = "Gift/sambo med ")]
        [DataType(DataType.Text)]
        public string? MarriedPartner { get; set; }

        [Display(Name = "barn 1 ")]
        [DataType(DataType.Text)]
        public string? ChildOne { get; set; }

        [Display(Name = "barn 2 ")]
        [DataType(DataType.Text)]
        public string? ChildTwo { get; set; }

        [Display(Name = "barn 3 ")]
        [DataType(DataType.Text)]
        public string? ChildThree { get; set; }

        [Display(Name = "barn 3 ")]
        [DataType(DataType.Text)]
        public string? ChildFour { get; set; }

        [Display(Name = "E-post ")]
        [DataType(DataType.EmailAddress)]
        public string? PrivateMail { get; set; }

        [Display(Name = "E-post arbete ")]
        [DataType(DataType.EmailAddress)]
        public string? WorkEMail { get; set; }

        [Display(Name = "E-post extra ")]
        [DataType(DataType.EmailAddress)]
        public string? ExtraMail { get; set; }

        [Display(Name = "Mobilnummer ")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Hemnummer ")]
        [DataType(DataType.PhoneNumber)]
        public string? HomePhoneNumber { get; set; }

        [Display(Name = "Arbets nummer ")]
        [DataType(DataType.PhoneNumber)]
        public string? WorkPhoneNumber { get; set; }

        [Display(Name = "Extra nummer ")]
        [DataType(DataType.PhoneNumber)]
        public string? ExtraPhoneNumber { get; set; }

        [Display(Name = "Hemsida ")]
        public string? HomePage { get; set; }

        [Display(Name = "Anteckningar ")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Display(Name = "Julkort ")]
        public bool ChristmasCard { get; set; }

        [Display(Name = "Släkting ")]
        public bool Relatives { get; set; }

        [Display(Name = "Vänner ")]
        public bool Friends { get; set; }

        [Display(Name = "Kollegor ")]
        public bool Colleagues { get; set; }
    }
}