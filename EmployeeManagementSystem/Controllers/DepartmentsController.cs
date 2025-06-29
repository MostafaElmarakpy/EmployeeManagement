using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: Departments
        public async Task<IActionResult> Index(string searchString)
        {
            IEnumerable<DepartmentViewModel> departments;

            //If search term is provided, filter the departments
            if (!string.IsNullOrEmpty(searchString))
            {
                departments = await _departmentService.SearchDepartmentsAsync(searchString);
            }
            else
            {
                departments = await _departmentService.GetAllDepartmentsAsync();
            }

            return View(departments);

        }

        // GET: Departments/Create
        public IActionResult CreateModal()
        {
            return PartialView("Create", new DepartmentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateModal(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.CreateDepartmentAsync(model);
                return Json(new { success = true });
            }
            return PartialView("Create", model);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> EditModal(int id)
        {
            var model = await _departmentService.GetDepartmentByIdAsync(id);
            if (model == null) return NotFound();
            return PartialView("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditModal(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentService.UpdateDepartmentAsync(model);
                    var updated = await _departmentService.GetDepartmentByIdAsync(model.Id);
                    return Json(new
                    {
                        success = true,
                        department = new
                        {
                            id = updated.Id,
                            name = updated.Name,
                            employeeCount = updated.EmployeeCount,
                            totalSalary = updated.TotalSalary.ToString("C")
                        }
                    });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Failed to update department. Please try again.");
                }
            }

            return PartialView("Edit", model);
        }

        public async Task<IActionResult> DeleteModal(int id)
        {
            var model = await _departmentService.GetDepartmentByIdAsync(id);
            if (model == null) return NotFound();
            return PartialView("Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModal(DepartmentViewModel model)
        {
            try
            {
                var canDelete = await _departmentService.CanDeleteDepartmentAsync(model.Id);
                if (!canDelete)
                {
                    return Json(new { success = false, message = "Cannot delete department with assigned employees." });
                }

                await _departmentService.DeleteDepartmentAsync(model.Id);
                return Json(new { success = true, id = model.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to delete department. Please try again." });
            }
        }
    }
}

