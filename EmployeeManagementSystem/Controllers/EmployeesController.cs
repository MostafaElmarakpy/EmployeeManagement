﻿
using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Authorize(Roles = "Admin,Manager")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeesController(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IWebHostEnvironment hostEnvironment)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            // If GetAllEmployeesAsync returns List<Employee>, map to List<EmployeeViewModel>
            var employeeViewModels = employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Salary = e.Salary,
                DepartmentId = e.DepartmentId,
                ManagerId = e.ManagerId,
                ImagePath = e.ImagePath
            }).ToList();

            //var employeeViewModels = _mapper.Map<List<EmployeeViewModel>>(employees);

            return View(employeeViewModels); 
            
        }
        [HttpGet]
        public async Task<IActionResult> CreateModal()
        {
            await PopulateViewBag();
            return PartialView("Create", new EmployeeViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateModal([FromForm] EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var saved = await _employeeService.CreateEmployeeAsync(model);

                if (saved != null)
                {
                    return Json(new
                    {
                        success = true,
                        employee = new
                        {
                            id = saved.Id,
                            firstName = saved.FirstName,
                            lastName = saved.LastName,
                            fullName = saved.FullName,
                            salary = saved.Salary.ToString("0.##"),
                            imagePath = saved.ImagePath,

                            departmentName = saved.DepartmentName,
                            //managerName = string.IsNullOrEmpty(saved.ManagerName) ? "-" : saved.ManagerName
                        }
                    });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create employee. Please try again.");
                }

            }

            await PopulateViewBag(model.DepartmentId, model.ManagerId);
            return PartialView("Create", model);
        }

        public async Task<IActionResult> GetEmployeeTableRows()
        {
            var employeeViewModels = await _employeeService.GetAllEmployeesAsync();
            return PartialView("EmployeeTableRows", employeeViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> EditModal(int id)
        {
            var model = await _employeeService.GetEmployeeByIdAsync(id);
            if (model == null) return NotFound();

            await PopulateViewBag(model.DepartmentId, model.ManagerId);
            return PartialView("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditModal([FromForm] EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.UpdateEmployeeAsync(model); // Fix: Removed assignment to a variable since UpdateEmployeeAsync returns void.

                return Json(new
                {
                    success = true,
                    employee = new
                    {
                        id = model.Id,
                        firstName = model.FirstName,
                        lastName = model.LastName,
                        fullName = model.FullName,
                        salary = model.Salary.ToString("0.##"),
                        imagePath = model.ImagePath,
                        departmentName = model.DepartmentName,
                        managerName = string.IsNullOrEmpty(model.ManagerName) ? "-" : model.ManagerName
                    }
                });
            }

            await PopulateViewBag(model.DepartmentId, model.ManagerId);
            return PartialView("Edit", model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteModal(int id)
        {
            var model = await _employeeService.GetEmployeeByIdAsync(id);
            if (model == null) return NotFound();

            return PartialView("Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModal(EmployeeViewModel model)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(model.Id);
            if (employee == null) return NotFound();

            if (!string.IsNullOrEmpty(employee.ImagePath))
            {
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath, employee.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            await _employeeService.DeleteEmployeeAsync(model.Id);
            return Json(new { success = true, id = model.Id });
        }

        private async Task PopulateViewBag(int? departmentId = null, int? managerId = null)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", departmentId);

            var employees = await _employeeService.GetAllEmployeesAsync();
            ViewBag.Managers = managerId.HasValue
                ? new SelectList(employees, "Id", "FullName", managerId)
                : new SelectList(employees, "Id", "FullName");
        }
    }
}


