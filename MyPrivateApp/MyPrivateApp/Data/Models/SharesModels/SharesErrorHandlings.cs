﻿using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.SharesModels
{
    public class SharesErrorHandlings
    {
        [Key]
        public int ErrorHandlingsId { get; set; }
        public string? Date { get; set; }
        public string? ErrorMessage { get; set; }
    }
}