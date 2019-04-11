using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BangazonWorkforce.Models.ViewModels {

    public class EmployeeTrainingEditViewModel {

        public EmployeeTrainingEditViewModel() {

            EnrolledTrainingPrograms = new List<TrainingProgram>();
            NotEnrolledTrainingPrograms = new List<TrainingProgram>();
        }

        public Employee Employee { get; set; }

       
        public List<TrainingProgram> EnrolledTrainingPrograms { get; set; }
        public List<TrainingProgram> NotEnrolledTrainingPrograms { get; set; }

        [Display(Name = "Currently Enrolled Training")]
        public List<string> EditedEnrolledTrainingPrograms { get; set; }

        [Display(Name = "Training Not Currently Enrolled")]
        public List<string> EditedNotEnrolledTrainingPrograms { get; set; }

        public List<SelectListItem> EnrolledTrainingProgramsOptions {
            get {
                return EnrolledTrainingPrograms.Select(t => new SelectListItem {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();
            }
        }

        public List<SelectListItem> NotEnrolledTrainingProgramsOptions {
            get {
                return NotEnrolledTrainingPrograms.Select(t => new SelectListItem {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();
            }
        }
    }
}
