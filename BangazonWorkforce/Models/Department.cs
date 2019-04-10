using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models {

    public class Department {


        public int Id { get; set;  }

        [Required]
        [StringLength(55, MinimumLength = 2)]
        [Display(Name = "Department Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Department Budget")]
        public int Budget { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
        
        [Display(Name = "Number of Employees")]
        public int EmployeeCount
        {
            get
            {
                return Employees.Count;
            }
        }
    }
}
