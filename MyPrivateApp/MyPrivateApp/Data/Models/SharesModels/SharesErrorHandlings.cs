using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesErrorHandlings
    {
        [Key]
        public int ErrorHandlingsId { get; set; }
        public string? Date { get; set; }
        public string? CompanyOrInformation { get; set; }
        public string? TypeOfTransaction { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Note { get; set; }
    }
}