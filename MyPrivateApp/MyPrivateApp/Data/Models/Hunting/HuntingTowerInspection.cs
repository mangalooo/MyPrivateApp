﻿
using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models.Hunting
{
    public class HuntingTowerInspection
    {
        [Key]
        public int HuntingTowerInspectionId { get; set; }
        public string? Date { get; set; }
        public HuntingPlaces Place { get; set; }
        public string? Number { get; set; }
        public HuntingTodo Todo { get; set; }
        public string? Note { get; set; }
    }
}
