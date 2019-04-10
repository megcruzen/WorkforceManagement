using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models {

    public class TrainingProgram {

        public int Id { get; set;  }

        [Required]
        [Display(Name = "Program Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End")]
        public DateTime EndDate { get; set; }

        [Required]
        public int MaxAttendees { get; set; }

        public List<Employee> Attendees { get; set; }
    }
}
