using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BangazonWorkforce.Models.ViewModels {

    public class EmployeeComputerEditViewModel {

        public EmployeeComputerEditViewModel() {

            Computers = new List<Computer>();
        }

        public Employee Employee { get; set; }
        public Computer Computer { get; set; }
        public List<Computer> Computers { get; set; }

        public List<SelectListItem> ComputerOptions {
            get {
                return Computers.Select(c => new SelectListItem {
                    Value = c.Id.ToString(),
                    Text = c.Make
                }).ToList();
            }
        }
    }
}
