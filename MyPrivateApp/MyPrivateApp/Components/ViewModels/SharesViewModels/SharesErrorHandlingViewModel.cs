﻿using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.SharesViewModels
{
    public class SharesErrorHandlingViewModel
    {
        public SharesErrorHandlingViewModel() => Date = DateTime.Now;

        public int ErrorHandlingsId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Företag / information")]
        [DataType(DataType.Text)]
        public string? CompanyOrInformation { get; set; }

        [Display(Name = "Transaktion")]
        [DataType(DataType.Text)]
        public string? TypeOfTransaction { get; set; }

        [Display(Name = "Felmeddelande")]
        [DataType(DataType.MultilineText)]
        public string? ErrorMessage { get; set; }

        [Display(Name = "Anteckningar")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}