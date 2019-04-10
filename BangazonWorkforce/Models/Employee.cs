// Author: Megan Cruzen

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(55, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(55, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName {
            get {
                return $"{FirstName} {LastName}";
            }
        }

        [Required]
        [Display(Name = "Supervisor")]
        public bool IsSupervisor { get; set; }

        [Required]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }
        
        public Department Department { get; set; }
        public Computer Computer { get; set; }

        [Display(Name = "Training")]
        public List<TrainingProgram> EmployeeTraining { get; set; }
    }
}