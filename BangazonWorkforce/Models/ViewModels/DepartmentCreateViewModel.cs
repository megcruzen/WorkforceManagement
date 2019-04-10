using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentCreateViewModel
    {
        [Display(Name = "Department Name")]
        public string Name { get; set; }

        [Display(Name = "Department Budget")]
        public int Budget { get; set; }
    }
}
