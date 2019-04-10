//Author: Cole Bryant

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Departments
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id AS DepartmentId, 
                                        d.Name AS DepartmentName, 
                                        d.Budget AS DepartmentBudget,
                                        e.Id AS EmployeeId,
                                        e.IsSupervisor AS IsSupervisor,
                                        e.FirstName AS EmployeeFirstName,
                                        e.LastName AS EmployeeLastName
                                        FROM Department d 
                                        LEFT JOIN Employee e ON d.Id = e.DepartmentId";
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, Department> departments = new Dictionary<int, Department>();

                    while (reader.Read())
                    {
                        int DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                        if (!departments.ContainsKey(DepartmentId))
                        {
                            Department department = new Department
                            {
                                Id = DepartmentId,
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Budget = reader.GetInt32(reader.GetOrdinal("DepartmentBudget"))
                            };
                            departments.Add(DepartmentId, department);
                        }
                            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                            {
                                Department currentDepartment = departments[DepartmentId];
                                currentDepartment.Employees.Add(
                                    new Employee
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                                        IsSupervisor = reader.GetBoolean((reader.GetOrdinal("IsSupervisor")))
                                    });
                            }
                    }
                    reader.Close();
                    return View(departments.Values.ToList());
                }
            }
        }

        // GET: Departments/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id AS DepartmentId, 
                                        d.Name AS DepartmentName, 
                                        d.Budget AS DepartmentBudget,
                                        e.Id AS EmployeeId,
                                        e.IsSupervisor AS IsSupervisor,
                                        e.FirstName AS EmployeeFirstName,
                                        e.LastName AS EmployeeLastName
                                        FROM Department d 
                                        LEFT JOIN Employee e ON d.Id = e.DepartmentId
                                        WHERE d.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Department department = null;

                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Budget = reader.GetInt32(reader.GetOrdinal("DepartmentBudget"))
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            if (!department.Employees.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("EmployeeId"))))
                            department.Employees.Add(
                                new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                                    IsSupervisor = reader.GetBoolean((reader.GetOrdinal("IsSupervisor")))
                                });
                        }
                    }
                    reader.Close();
                    return View(department);
                }
            }
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            DepartmentCreateViewModel viewModel = new DepartmentCreateViewModel();

            return View(viewModel);
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Department (Name, Budget)
                                            VALUES (@name, @budget)";
                        cmd.Parameters.Add(new SqlParameter("@name", viewModel.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", viewModel.Budget));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Departments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}