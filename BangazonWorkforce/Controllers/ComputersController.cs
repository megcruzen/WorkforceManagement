// Author: Cole Bryant. Purpose: This is the controller for computers in the database and allows for users
// to view a list of computers, view details associated with each computer, add a new computer and assign to an
// employee, and delete a computer from the database.

// Search added by Megan Cruzen

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
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        // GET: Computers
        public ActionResult Index(string searchString)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id,
                                        c.Make,
                                        c.Manufacturer,
                                        c.PurchaseDate,
										e.Id AS EmployeeId,
										e.FirstName,
                                        e.LastName
                                        FROM Computer c
                                        LEFT JOIN (SELECT * 
										FROM ComputerEmployee
										WHERE UnassignDate IS NULL)
										ce ON c.Id = ce.ComputerId
                                        LEFT JOIN Employee e ON ce.EmployeeId = e.Id
                                        WHERE 1=1";

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        cmd.CommandText += @" AND
                                             (c.Make LIKE @searchString OR c.Manufacturer LIKE @searchString)";
                        cmd.Parameters.Add(new SqlParameter("@searchString", $"%{searchString}%"));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Employee = new Employee()
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            computer.Employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        }
                        computers.Add(computer);
                    }
                    
                    reader.Close();
                    return View(computers);
                }
            }
        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id,
                                        c.Make,
                                        c.Manufacturer,
                                        c.PurchaseDate,
										e.Id AS EmployeeId,
										e.FirstName,
                                        e.LastName
                                        FROM Computer c
                                        LEFT JOIN (SELECT * 
										FROM ComputerEmployee
										WHERE UnassignDate IS NULL)
										ce ON c.Id = ce.ComputerId
                                        LEFT JOIN Employee e ON ce.EmployeeId = e.Id
                                        WHERE c.Id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Computer computer = null;

                    while (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Employee = new Employee()
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            computer.Employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        };
                    }
                    reader.Close();
                    return View(computer);
                }
            }
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            ComputerCreateViewModel viewModel = new ComputerCreateViewModel
            {
                Employees = GetAllUnassignedEmployees()
            };
            return View(viewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer (Make, Manufacturer, PurchaseDate)
                                            VALUES (@make, @manufacturer, @purchaseDate)";
                        cmd.Parameters.Add(new SqlParameter("@make", viewModel.Computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", viewModel.Computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", viewModel.Computer.PurchaseDate));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Computers/Delete/5
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

        // Get all employees that are not assigned to a computer
        private List<Employee> GetAllUnassignedEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id,
                                        e.FirstName,
                                        e.LastName,
                                        ce.Id
                                        FROM Employee e
                                        LEFT JOIN (SELECT * FROM ComputerEmployee 
                                        WHERE UnassignDate IS NULL)ce ON e.Id = ce.ComputerId
                                        WHERE ce.Id IS NULL;";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        int employeeId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!employees.Exists(emp => emp.Id == employeeId ))
                        {
                            Employee employee = new Employee
                            {
                                Id = employeeId,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                            employees.Add(employee);
                        }
                    }
                    reader.Close();
                    return employees;
                }
            }
        }
    }
}