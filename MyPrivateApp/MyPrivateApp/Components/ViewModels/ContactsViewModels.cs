using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Client.ViewModels
{
    public sealed class ContactsViewModels
    {
        [Key]
        public int ContactsId { get; set; }

        [Required(ErrorMessage = "Skriv in personens namn:")]
        [Display(Name = "Namn: ")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Skriv in personens fördelsedag::")]
        [Display(Name = "Födelsedag: ")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Skriv in personens adress::")]
        [Display(Name = "Adress: ")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Skriv in personens postnummer:")]
        [Display(Name = "Postnummer: ")]
        [DataType(DataType.PostalCode)]
        public int PostCode { get; set; }

        [Required(ErrorMessage = "Skriv in personens stad:")]
        [Display(Name = "Stad: ")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? City { get; set; }

        [Display(Name = "E-post: ")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string? PrivateMail { get; set; }

        [Display(Name = "E-post arbete: ")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string? WorkEMail { get; set; }

        [Display(Name = "E-post extra: ")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string? ExtraMail { get; set; }

        [Display(Name = "Mobilnummer: ")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Hemnummer: ")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? HomePhoneNumber { get; set; }

        [Display(Name = "Arbets nummer: ")]
        [DataType(DataType.PhoneNumber)]
        public string? WorkPhoneNumber { get; set; }

        [Display(Name = "Extra nummer: ")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string? ExtraPhoneNumber { get; set; }

        [Display(Name = "Hemsida: ")]
        public string? HomePage { get; set; }

        [Display(Name = "Anteckningar: ")]
        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Julkort: ")]
        public bool ChristmasCard { get; set; }

        [Display(Name = "Släkting: ")]
        public bool Relatives { get; set; }

        [Display(Name = "Vänner: ")]
        public bool Friends { get; set; }

        [Display(Name = "Kollegor: ")]
        public bool Colleagues { get; set; }
    }
}