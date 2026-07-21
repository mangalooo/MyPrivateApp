using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class ShopingList
    {
        [Key]
        public int ShopingListId { get; set; }

        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [DataType(DataType.Date)]
        [StringLength(50)]
        public string? Date { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Place { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(2000)]
        public string? List { get; set; }
    }
}