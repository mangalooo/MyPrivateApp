﻿using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Components.ViewModels.HuntingViemModels
{
    public class HuntingPreyViewModels
    {
        public HuntingPreyViewModels() => Date = DateTime.Now;

        [Key]
        public int HuntingPreyId { get; set; }

        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Du måste skriva vem som sköt!")]
        [Display(Name = "Jägare")]
        [DataType(DataType.Text)]
        public string? Hunter { get; set; }

        [Required(ErrorMessage = "Du måste skriva vilket vilt!")]
        [Display(Name = "Jakt typ")]
        public HuntingForm HuntingForm { get; set; }

        [Required(ErrorMessage = "Du måste skriva vilket vilt!")]
        [Display(Name = "Vilt")]
        public WildAnimal WildAnimal { get; set; }

        [Required(ErrorMessage = "Du måste skriva vilken typ av vilt!")]
        [Display(Name = "Typ")]
        [DataType(DataType.Text)]
        public string? Type { get; set; }

        [Display(Name = "Pass")]
        [DataType(DataType.Text)]
        public string? HuntingPass { get; set; }

        [Display(Name = "Hund")]
        [DataType(DataType.Text)]
        public string? Dog { get; set; }

        [Required(ErrorMessage = "Du måste skriva var djuret sköts!")]
        [Display(Name = "Plats")]
        [DataType(DataType.Text)]
        public HuntingPlaces HuntingPlaces { get; set; }

        [Display(Name = "Beskrivning")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}