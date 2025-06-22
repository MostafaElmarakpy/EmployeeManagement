using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            try
            {
                IEnumerable<DepartmentViewModel> departments;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    departments = await _departmentService.SearchDepartmentsAsync(searchTerm);
                }
                else
                {
                    departments = await _departmentService.GetAllDepartmentsAsync();
                }

                ViewBag.SearchTerm = searchTerm;
                return View(departments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load departments. Please try again.";
                return View(new List<DepartmentViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                var departments = await _departmentService.SearchDepartmentsAsync(searchTerm);
                return PartialView("_DepartmentTableRows", departments);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Search failed. Please try again." });
            }
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
            return PartialView("_Create", model);
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

