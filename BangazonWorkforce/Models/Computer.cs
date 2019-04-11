﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models {

    public class Computer {

        public int Id { get; set;  }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public DateTime? DecomissionDate { get; set; }

        [Required]
        [Display (Name = "Computer")]
        public string Make { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        public Employee Employee { get; set; }
    }
}
