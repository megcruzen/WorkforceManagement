using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class ComputerCreateViewModel
    {
        public Computer Computer { get; set; }

        public int EmployeeId { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();

        public List<SelectListItem> EmployeeOptions
        {
            get
            {
                return Employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                }).ToList();
            }
        }
    }
}
