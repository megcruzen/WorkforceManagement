using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BangazonWorkforce.Models;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models.ViewModels;
using System;

namespace BangazonWorkforce.Controllers {

    public class EmployeesController : Controller {

        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config) {
            _config = config;
        }

        public SqlConnection Connection {
            get {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employees
        public ActionResult Index() {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, e.IsSupervisor, 
                                               e.DepartmentId, d.[Name] as DeptName 
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.id = e.DepartmentId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read()) {

                        Employee newEmployee = new Employee {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DeptName"))
                            }
                        };

                        employees.Add(newEmployee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id) {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, 
                                                e.IsSupervisor, e.DepartmentId, d.[Name] as DeptName, 
                                                c.Id AS ComputerId, c.Make, c.Manufacturer, 
                                                ce.AssignDate, ce.UnassignDate, tp.id AS TrainingProgramId, 
                                                tp.[Name] AS TrainingProgramName, tp.StartDate, tp.EndDate,
                                                tp.MaxAttendees
                                           FROM Employee e
                                      LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId
                                      LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                      LEFT JOIN Department d ON d.id = e.DepartmentId
                                      LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id
                                      LEFT JOIN Computer c ON c.Id = ce.ComputerId
                                          WHERE e.Id = @id AND UnassignDate IS NULL;";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    while (reader.Read()) {

                        if (employee == null) {

                            employee = new Employee {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Department = new Department {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                },
                                Computer = new Computer(),
                                EmployeeTraining = new List<TrainingProgram>()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId"))) {

                            if (employee.Computer.Make == null) {

                                employee.Computer = new Computer {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                };
                                
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId"))) {

                            int trainingProgramId = reader.GetInt32(reader.GetOrdinal("TrainingProgramId"));

                            if (!employee.EmployeeTraining.Any(tp => tp.Id == trainingProgramId)) {

                                TrainingProgram program = new TrainingProgram {
                                    Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                    Name = reader.GetString(reader.GetOrdinal("TrainingProgramName")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                    MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                                };
                                employee.EmployeeTraining.Add(program);
                            }
                        }
                    }
                    reader.Close();
                    return View(employee);
                }
            }
        }

        // GET: Employees/Create
        public ActionResult Create() {

            EmployeeCreateViewModel viewModel =
                new EmployeeCreateViewModel(_config.GetConnectionString("DefaultConnection"));
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel viewModel) {

            try {
                using (SqlConnection conn = Connection) {

                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand()) {

                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, IsSupervisor, DepartmentId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstname, @lastname, @supervisor, @deptId);
                                            SELECT MAX(Id) 
                                              FROM Employee";

                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@supervisor", viewModel.Employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@deptId", viewModel.Employee.DepartmentId));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }

            } catch {

                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id) {

            EmployeeEditViewModel viewModel = new EmployeeEditViewModel {

                EmployeeTrainingEditViewModel = new EmployeeTrainingEditViewModel {
                    EnrolledTrainingPrograms = GetEnrolledTrainingPrograms(id),
                    NotEnrolledTrainingPrograms = GetNotEnrolledTrainingPrograms(id),
                    EditedEnrolledTrainingPrograms = null,
                    EditedNotEnrolledTrainingPrograms = null
                },
                EmployeeDataAndDeptEditViewModel = new EmployeeDataAndDeptEditViewModel {
                    Employee = GetEmployeeById(id),
                    Departments = GetAllDepartments()
                },
                EmployeeComputerEditViewModel = new EmployeeComputerEditViewModel {
                    Computers = GetUnassignedComputers(id)
                }
            };

            return View(viewModel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel viewModel) {

            Employee Employee = GetEmployeeById(id);

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {
                    int NewComputerId = int.Parse(viewModel.EmployeeComputerEditViewModel.NewComputerId);
                    int PreviousComptuerId = Employee.Computer.Id;

                    cmd.CommandText = @"UPDATE Employee 
                                            SET firstname = @firstname, 
                                                lastname = @lastname,
                                                isSupervisor = @isSupervisor, 
                                                departmentId = @departmentId
                                            WHERE id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.EmployeeDataAndDeptEditViewModel.Employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.EmployeeDataAndDeptEditViewModel.Employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@isSupervisor", viewModel.EmployeeDataAndDeptEditViewModel.Employee.IsSupervisor));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", viewModel.EmployeeDataAndDeptEditViewModel.Employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();

                    if (NewComputerId != 0 && NewComputerId != PreviousComptuerId) {

                        UpdateEmployeeComputer(id, NewComputerId, PreviousComptuerId);
                    }

                    UpdateEmployeeTrainingPrograms(id, viewModel);

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        private void UpdateEmployeeComputer(int employeeId, int newComputerId, int previousComputerId) {

            DateTime today = DateTime.UtcNow;
            string unassignDate = $"{today.Year}-{today.Month}-{today.Day}";

            using (SqlConnection conn = Connection) {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = @"INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate)
                                             OUTPUT INSERTED.Id
                                             VALUES (@employeeId, @computerId, @assignDate);
                                             SELECT MAX(Id) 
                                               FROM ComputerEmployee;";
                    cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
                    cmd.Parameters.Add(new SqlParameter("@computerId", newComputerId));
                    cmd.Parameters.Add(new SqlParameter("@assignDate", unassignDate));

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"UPDATE ComputerEmployee
                                           SET EmployeeId = @employeeId,
                                               ComputerId = @computerId2,
                                               UnassignDate = @unassignDate
                                         WHERE EmployeeId = @employeeId AND ComputerId = @computerId2;";

                    cmd.Parameters.Add(new SqlParameter("@computerId2", previousComputerId));
                    cmd.Parameters.Add(new SqlParameter("@unassignDate", unassignDate));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateEmployeeTrainingPrograms(int id, EmployeeEditViewModel viewModel) {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    if (viewModel.EmployeeTrainingEditViewModel.EditedEnrolledTrainingPrograms != null) {

                        foreach (var item in viewModel.EmployeeTrainingEditViewModel.EditedEnrolledTrainingPrograms) {

                            int EmployeeIdToRemove = id;
                            int TrainingProgramIdToRemove = int.Parse(item);

                            cmd.CommandText = $@"DELETE FROM EmployeeTraining 
                                                      WHERE TrainingProgramId = {TrainingProgramIdToRemove}
                                                            AND EmployeeId = {EmployeeIdToRemove};";

                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (viewModel.EmployeeTrainingEditViewModel.EditedNotEnrolledTrainingPrograms != null) {

                        foreach (var item in viewModel.EmployeeTrainingEditViewModel.EditedNotEnrolledTrainingPrograms) {

                            int EmployeeIdToAdd = id;
                            int TrainingProgramIdToAdd = int.Parse(item);

                            cmd.CommandText = $@"INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId)
                                                 OUTPUT INSERTED.Id
                                                 VALUES ({EmployeeIdToAdd}, {TrainingProgramIdToAdd});
                                                 SELECT MAX(Id) 
                                                   FROM EmployeeTraining;";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        private List<Computer> GetUnassignedComputers(int id) {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT c.id AS ComputerId, c.Make, c.Manufacturer
                                          FROM Computer c
                                     LEFT JOIN ComputerEmployee ce ON c.id = ce.ComputerId
                                         WHERE ce.EmployeeId = {id} AND ce.UnassignDate IS NULL";

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> Computers = new List<Computer>();

                    if (reader.Read()) {

                        string make = $"{reader.GetString(reader.GetOrdinal("Make"))}***";

                        Computers.Add(new Computer {
                            Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                            Make = make,
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        });
                    }
                    reader.Close();

                    cmd.CommandText = @"SELECT com.Id, com.Make, com.Manufacturer
                                          FROM Computer com
                                     LEFT JOIN (SELECT c.id, count(*) AS CountNulls
			                                      FROM Computer c
		                                     LEFT JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
			                                     WHERE UnassignDate IS NULL
		                                      GROUP BY c.Id) cc ON com.Id = cc.Id
                                         WHERE cc.CountNulls IS NULL;";

                    SqlDataReader reader2 = cmd.ExecuteReader();

                    while (reader2.Read()) {
                        Computers.Add(new Computer {
                            Id = reader2.GetInt32(reader2.GetOrdinal("id")),
                            Make = reader2.GetString(reader2.GetOrdinal("Make")),
                            Manufacturer = reader2.GetString(reader2.GetOrdinal("Manufacturer"))
                        });
                    }
                    reader2.Close();
                    return Computers;
                }
            }
        }

        private List<TrainingProgram> GetEnrolledTrainingPrograms(int id) {

            DateTime today = DateTime.UtcNow;
            string filterDate = $"{today.Year}-{today.Month}-{today.Day}";

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = $@"SELECT tp.Id AS ProgramId, tp.[Name] AS ProgramName
                                           FROM EmployeeTraining et
                                      LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                          WHERE et.EmployeeId = {id} AND StartDate >= '{filterDate}'";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> TrainingPrograms = new List<TrainingProgram>();

                    while (reader.Read()) {
                        TrainingPrograms.Add(new TrainingProgram {
                            Id = reader.GetInt32(reader.GetOrdinal("ProgramId")),
                            Name = reader.GetString(reader.GetOrdinal("ProgramName"))
                        });
                    }
                    reader.Close();
                    return TrainingPrograms;
                }
            }
        }

        private List<TrainingProgram> GetNotEnrolledTrainingPrograms(int id) {

            DateTime today = DateTime.UtcNow;
            string filterDate = $"{today.Year}-{today.Month}-{today.Day}";

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {
                    
                    cmd.CommandText = $@"SELECT DISTINCT tp.Id AS ProgramId, tp.[Name] AS ProgramName
                                                    FROM TrainingProgram tp
                                               LEFT JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
                                                   WHERE tp.[Name] NOT IN (SELECT DISTINCT tp.Name 
							                                  FROM EmployeeTraining et
							                             LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
							                                 WHERE et.EmployeeId = {id} AND StartDate >= '{filterDate}')";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> TrainingPrograms = new List<TrainingProgram>();

                    while (reader.Read()) {
                        TrainingPrograms.Add(new TrainingProgram {
                            Id = reader.GetInt32(reader.GetOrdinal("ProgramId")),
                            Name = reader.GetString(reader.GetOrdinal("ProgramName"))
                        });
                    }
                    reader.Close();
                    return TrainingPrograms;
                }
            }
        }

        private List<Department> GetAllDepartments() {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = @"SELECT id, Name, Budget FROM Department;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> Departments = new List<Department>();

                    while (reader.Read()) {
                        Departments.Add(new Department {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Employees = new List<Employee>()
                        });
                    }
                    reader.Close();
                    return Departments;
                }
            }
        }

        private Employee GetEmployeeById(int id) {

            using (SqlConnection conn = Connection) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, 
                                                e.IsSupervisor, e.DepartmentId, d.[Name] as DeptName, 
                                                c.Id AS ComputerId, c.Make, c.Manufacturer, 
                                                ce.AssignDate, ce.UnassignDate, tp.id AS TrainingProgramId, 
                                                tp.[Name] AS TrainingProgramName, tp.StartDate, tp.EndDate,
                                                tp.MaxAttendees
                                           FROM Employee e
                                      LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId
                                      LEFT JOIN TrainingProgram tp ON tp.Id = et.TrainingProgramId
                                      LEFT JOIN Department d ON d.id = e.DepartmentId
                                      LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id AND UnassignDate IS NULL
                                      LEFT JOIN Computer c ON c.Id = ce.ComputerId
                                          WHERE e.Id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;
                    if (reader.Read()) {
                        if (employee == null) {

                            employee = new Employee {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Department = new Department {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("DeptName"))
                                },
                                Computer = new Computer(),
                                EmployeeTraining = new List<TrainingProgram>()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId"))) {

                            if (employee.Computer.Make == null) {

                                employee.Computer = new Computer {
                                    Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                };

                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId"))) {

                            int trainingProgramId = reader.GetInt32(reader.GetOrdinal("TrainingProgramId"));

                            if (!employee.EmployeeTraining.Any(tp => tp.Id == trainingProgramId)) {

                                TrainingProgram program = new TrainingProgram {
                                    Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                    Name = reader.GetString(reader.GetOrdinal("TrainingProgramName")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                    MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                                };
                                employee.EmployeeTraining.Add(program);
                            }
                        }
                    }
                    reader.Close();
                    return employee;
                }
            }
        }
    }
}
/*

#4 HR should be able to edit an employee

    Acceptance Criteria
        Given user is viewing an employee
        When user clicks on the Edit link
        Then user should be able to edit the last name of the employee
        Or change the department to which the employee is assigned
        Or change the computer assigned to the employee
        Or add/remove training programs for the employee to attend in the future
*/
