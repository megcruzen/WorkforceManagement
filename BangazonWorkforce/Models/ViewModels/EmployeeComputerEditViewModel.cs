using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BangazonWorkforce.Models.ViewModels {

    public class EmployeeComputerEditViewModel {

        public EmployeeComputerEditViewModel() {

            Computers = new List<Computer>();
        }

        public Employee Employee { get; set; }
        public Computer Computer { get; set; }
        public List<Computer> Computers { get; set; }

        [Display (Name = "Computer")]
        public string NewComputerId { get; set; }

        public List<SelectListItem> ComputerOptions {
            get {
                List<SelectListItem> computerList = Computers.Select(c => new SelectListItem {
                    Value = c.Id.ToString(),
                    Text = c.Make
                }).ToList();

                computerList.Insert(0, new SelectListItem { Value = "0", Text = "Select A Computer", Selected = true });

                return computerList;
            }
        }
    }
}
