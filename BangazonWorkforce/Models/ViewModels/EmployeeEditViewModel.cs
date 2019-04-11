namespace BangazonWorkforce.Models.ViewModels {

    public class EmployeeEditViewModel {

        public EmployeeEditViewModel() {

            EmployeeTrainingEditViewModel = new EmployeeTrainingEditViewModel();
            EmployeeComputerEditViewModel = new EmployeeComputerEditViewModel();
            EmployeeDataAndDeptEditViewModel = new EmployeeDataAndDeptEditViewModel();
        }

        public EmployeeTrainingEditViewModel EmployeeTrainingEditViewModel { get; set; }
        public EmployeeComputerEditViewModel EmployeeComputerEditViewModel { get; set; }
        public EmployeeDataAndDeptEditViewModel EmployeeDataAndDeptEditViewModel { get; set; }
    }
}
