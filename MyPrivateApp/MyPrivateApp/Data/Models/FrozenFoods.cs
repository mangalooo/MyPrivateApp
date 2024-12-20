﻿using MyPrivateApp.Components.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyPrivateApp.Data.Models
{
    public class FrozenFoods
    {
        [Key]
        public int FrozenFoodsId { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Date { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public string? Type { get; set; }

        public int Number { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50)]
        public FreezerPlaces Place { get; set; } //Frys platser

        [DataType(DataType.Text)]
        [StringLength(50)]
        public FreezerCompartment FreezerCompartment { get; set; } //Frysfack 

        [DataType(DataType.Text)]
        [StringLength(50)]
        public FreezerFrozenGoods FrozenGoods { get; set; } //Frysvaror

        public double Weight { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}