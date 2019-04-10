using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels {

    public class EmployeeEditViewModel {

        public EmployeeEditViewModel() {

            Departments = new List<Department>();
            Computers = new List<Computer>();
            TrainingPrograms = new List<TrainingProgram>();
        }

        public EmployeeEditViewModel(string connectionString) {

            using (SqlConnection conn = new SqlConnection(connectionString)) {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) {

                    cmd.CommandText = @"SELECT id, Name, Budget FROM Department;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    Departments = new List<Department>();

                    while (reader.Read()) {
                        Departments.Add(new Department {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Employees = new List<Employee>()
                        });
                    }
                    reader.Close();

                    cmd.CommandText = @"SELECT id, Make, Manufacturer FROM Computer;";

                    SqlDataReader reader2 = cmd.ExecuteReader();

                    while (reader2.Read()) {
                        Computers.Add(new Computer {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        });
                    }
                    reader2.Close();

                    cmd.CommandText = @"SELECT id, Name FROM TrainingProgram;";

                    SqlDataReader reader3 = cmd.ExecuteReader();

                    while (reader2.Read()) {
                        TrainingPrograms.Add(new TrainingProgram {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                    reader3.Close();
                }
            }
        }

        public Employee Employee { get; set; }
        public Computer Computer { get; set; }
        public List<Department> Departments { get; set; }
        public List<Computer> Computers { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; }

        public List<SelectListItem> DepartmentOptions {
            get {
                return Departments.Select(d => new SelectListItem {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }

        public List<SelectListItem> ComputerOptions {
            get {
                return Computers.Select(c => new SelectListItem {
                    Value = c.Id.ToString(),
                    Text = c.Make
                }).ToList();
            }
        }

        public List<SelectListItem> TrainingProgramsOptions {
            get {
                return TrainingPrograms.Select(t => new SelectListItem {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();
            }
        }
    }
}
