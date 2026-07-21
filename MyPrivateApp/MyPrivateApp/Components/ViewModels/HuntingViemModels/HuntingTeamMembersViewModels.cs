
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.HuntingViemModels
{
    public class HuntingTeamMembersViewModels
    {
        [Key]
        public int HuntingTeamMembersId { get; set; }

        [Required(ErrorMessage = "Du måste skriva ett namn!")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Birthday { get; set; }

        [DataType(DataType.Text)]
        public string? Address { get; set; }

        [DataType(DataType.PostalCode)]
        public string? PostCode { get; set; }

        [DataType(DataType.Text)]
        public string? City { get; set; }

        [Required(ErrorMessage = "Du måste skriva en e-post adress!")]
        [DataType(DataType.EmailAddress)]
        public string? Mail { get; set; }

        [Required(ErrorMessage = "Du måste skriva ett mobilnummer!")]
        [DataType(DataType.PhoneNumber)]
        public string? MobilePhone { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}