﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BangazonWorkforce.Models;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models.ViewModels;

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

                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, e.IsSuperVisor, 
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
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
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

                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, e.IsSuperVisor, 
                                               e.DepartmentId, d.[Name] as DeptName 
                                         FROM Employee e
                                    LEFT JOIN Department d ON d.id = e.DepartmentId
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    if (reader.Read()) {

                        employee = new Employee {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DeptName"))
                            }
                        };
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

                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, IsSuperVisor, DepartmentId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstname, @lastname, @supervisor, @deptId);
                                            SELECT MAX(Id) 
                                              FROM Employee";

                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@supervisor", viewModel.Employee.IsSuperVisor));
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
            return View();
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            } catch {

                return View();
            }
        }
    }
}
/*

#2  HR should be able to add an employee 

    Acceptance Criteria
        Given the user is viewing the list of employees
        When the user clicks the Create New link
        Then a form for be displayed on which the following information can be entered
            1. First name
            2. Last name
            3. Is the employee a supervisor
            4. Select a department from a drop down

#3  HR should be able to view employee details

    Acceptance Criteria
        Given a user is viewing the employee list
        When the user clicks on an individual employee
        Then the user should be shown a detail view of that employee, 
        and it must contain the following information:
            1. First name and last name
            2. Department
            3. Currently assigned computer
            4. Training programs they have attended, or plan on attending

#4 HR should be able to edit an employee

    Acceptance Criteria
        Given user is viewing an employee
        When user clicks on the Edit link
        Then user should be able to edit the last name of the employee
        Or change the department to which the employee is assigned
        Or change the computer assigned to the employee
        Or add/remove training programs for the employee to attend in the future
*/
