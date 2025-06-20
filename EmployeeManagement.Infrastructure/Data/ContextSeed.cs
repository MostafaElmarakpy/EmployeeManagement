using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Data
{
    public class ContextSeed
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContextSeed(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            // Apply migrations
            //await _context.Database.MigrateAsync();

            // Seed roles
            await SeedRolesAsync();


            // Seed users
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("Manager"))
                await _roleManager.CreateAsync(new IdentityRole("Manager"));

            if (!await _roleManager.RoleExistsAsync("Employee"))
                await _roleManager.CreateAsync(new IdentityRole("Employee"));
        }
/*
        private async Task SeedUsersAsync()
        {
            try
            {
                if (await _userManager.FindByNameAsync("admin") == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                        FirstName = "Admin",
                        LastName = "User"
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Create manager user + employee record
                ApplicationUser? managerUser = await _userManager.FindByNameAsync("manager");
                if (managerUser == null)
                {
                    managerUser = new ApplicationUser
                    {
                        UserName = "manager",
                        Email = "manager@example.com",
                        EmailConfirmed = true,
                        FirstName = "Manager",
                        LastName = "User"
                    };

                    var result = await _userManager.CreateAsync(managerUser, "Manager@123");
                    if (result.Succeeded)
                        await _userManager.AddToRoleAsync(managerUser, "Manager");
                }

                //Department? department = null;
                if (!_context.Departments.Any())
                {
                var departmentData = File.ReadAllText("../EmployeeManagement.Infrastructure/Data/DataSeed/Departments.json");
                    var departments = JsonSerializer.Deserialize<List<Department>>(departmentData);
                    await _context.Departments.AddRangeAsync(departments);
                    await _context.SaveChangesAsync();
                }


                //// Create Manager Employee Record
                //if (!_context.Employees.Any(e => e.UserId == managerUser.Id))
                //{
                //    var managerEmployee = new Employee
                //    {
                //        FirstName = "Manager",
                //        LastName = "User",
                //        Salary = 15000, 
                //        DepartmentId = department.Id,
                //        IsActive = true,
                //        CreatedAt= DateTime.UtcNow,
                //        UserId = managerUser.Id,
                //    };
                //    _context.Employees.Add(managerEmployee);
                //    await _context.SaveChangesAsync();
                //}

                // Get manager employee to link employees
                var managerEmployeeEntity = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == managerUser.Id);

                // Seed employees if none exist
                if (!_context.Employees.Any())
                {
                    var employeesData = File.ReadAllText("../EmployeeManagement.Infrastructure/Data/DataSeed/Employees.json");
                    var employeeDtos = JsonSerializer.Deserialize<List<Employee>>(employeesData,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (employeeDtos == null)
                    {
                        throw new Exception("Failed to deserialize employees data");
                    }

                    var employees = employeeDtos.Select(dto => new Employee
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Salary = dto.Salary,
                        ImagePath = dto.ImagePath,
                        DepartmentId = dto.DepartmentId,
                        ManagerId = dto.ManagerId,
                        IsActive = dto.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        ManagedEmployees = new List<Employee>(),
                        EmployeeTasks = new List<EmployeeTask>()
                    }).ToList();

                    await _context.Employees.AddRangeAsync(employees);
                    await _context.SaveChangesAsync();
                }
            }
            catch (JsonException ex)
            {
                throw new Exception("Error parsing employees JSON data", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error saving employees to database", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while seeding employees", ex);
            }
        


        }
        */
      

        private async Task SeedUsersAsync()
        {
            try
            {
                // First ensure departments exist
                if (!_context.Departments.Any())
                {
                    var departmentData = File.ReadAllText("../EmployeeManagement.Infrastructure/Data/DataSeed/Departments.json");
                    var departments = JsonSerializer.Deserialize<List<Department>>(departmentData,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (departments == null)
                        throw new Exception("Failed to deserialize departments data");

                    foreach (var dept in departments)
                    {
                        dept.CreatedAt = DateTime.UtcNow;
                        dept.IsActive = true;
                    }

                    await _context.Departments.AddRangeAsync(departments);
                    await _context.SaveChangesAsync();
                }

                // Create manager user if doesn't exist
                ApplicationUser? managerUser = await _userManager.FindByNameAsync("manager");
                if (managerUser == null)
                {
                    managerUser = new ApplicationUser
                    {
                        UserName = "manager",
                        Email = "manager@example.com",
                        EmailConfirmed = true,
                        FirstName = "Manager",
                        LastName = "User",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    var result = await _userManager.CreateAsync(managerUser, "Manager@123");
                    if (result.Succeeded)
                        await _userManager.AddToRoleAsync(managerUser, "Manager");
                }

                // Create manager employee if doesn't exist
                var managerEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == managerUser.Id);

                if (managerEmployee == null)
                {
                    var firstDepartment = await _context.Departments.FirstAsync();
                    managerEmployee = new Employee
                    {
                        FirstName = managerUser.FirstName!,
                        LastName = managerUser.LastName!,
                        Salary = 15000,
                        DepartmentId = firstDepartment.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UserId = managerUser.Id
                    };

                    await _context.Employees.AddAsync(managerEmployee);
                    await _context.SaveChangesAsync();
                }

                // Seed regular employees if none exist
                if (!_context.Employees.Any(e => e.UserId == null))
                {
                    var employeesData = File.ReadAllText("../EmployeeManagement.Infrastructure/Data/DataSeed/Employees.json");
                    var employees = JsonSerializer.Deserialize<List<Employee>>(employeesData,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (employees == null)
                        throw new Exception("Failed to deserialize employees data");

                    var departments = await _context.Departments.ToListAsync();

                    foreach (var employee in employees)
                    {
                        // Ensure we map to existing department
                        var departmentIndex = ((employee.DepartmentId - 1) % departments.Count);
                        employee.DepartmentId = departments[departmentIndex].Id;
                        employee.ManagerId = managerEmployee.Id;
                        employee.IsActive = true;
                        employee.CreatedAt = DateTime.UtcNow;
                        


                    }

                    await _context.Employees.AddRangeAsync(employees);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error seeding data: {ex.Message}", ex);
            }
        }


    }




}

