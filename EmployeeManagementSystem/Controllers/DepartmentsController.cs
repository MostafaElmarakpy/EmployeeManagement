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
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<DepartmentViewModel> departments = await _departmentService.GetAllDepartmentsAsync();
                return View(departments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load departments. Please try again.";
                return View(new List<DepartmentViewModel>());
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
                await _departmentService.UpdateDepartmentAsync(model);
                return Json(new { success = true });
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
            var department = await _departmentService.GetDepartmentByIdAsync(model.Id);
            if (department == null) return NotFound();

            var hasEmployees = await _departmentService.GetDepartmentsWithEmployeesAsync();
            if (hasEmployees.Any(d => d.Id == model.Id))
            {
                ModelState.AddModelError("", "Cannot delete department with assigned employees.");
                return PartialView("Delete", model);
            }

            await _departmentService.DeleteDepartmentAsync(model.Id);
            return Json(new { success = true });
        }
    }
}