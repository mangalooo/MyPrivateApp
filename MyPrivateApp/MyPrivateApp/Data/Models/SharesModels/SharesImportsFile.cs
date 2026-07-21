using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesImportsFile
    {
        [Key]
        public int SharesImportsFileId { get; set; }
        public string? Date { get; set; }
        public string? FileName { get; set; }
        public int NumbersOfTransaction { get; set; }
        public string? Errors { get; set; }
        public string? Note { get; set; }
    }
}