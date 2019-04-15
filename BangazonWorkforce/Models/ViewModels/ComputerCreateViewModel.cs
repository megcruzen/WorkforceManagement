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

        public int? EmployeeId { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();

        public List<SelectListItem> EmployeeOptions
        {
            get
            {
                List<SelectListItem> EmployeeList = Employees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                }).ToList();
                EmployeeList.Insert(0, new SelectListItem{
                    Value= null,
                    Text = "No Assignee",
                    Selected = true
                });
                return EmployeeList;
                
            }
        }
    }
}
