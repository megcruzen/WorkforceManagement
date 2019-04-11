using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models.ViewModels {

    public class EmployeeDataAndDeptEditViewModel {

        public EmployeeDataAndDeptEditViewModel() {

            Departments = new List<Department>();
            Employee = new Employee();
        }

        public Employee Employee { get; set; }
        public List<Department> Departments { get; set; }

        public List<SelectListItem> DepartmentOptions {
            get {
                return Departments.Select(d => new SelectListItem {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }
    }
}
