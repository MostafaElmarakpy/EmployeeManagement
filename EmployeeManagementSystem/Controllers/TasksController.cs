using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskStatus = EmployeeManagement.Domain.Models.TaskStatus;

namespace EmployeeManagement.Controllers
{
    //[Authorize]
    public class TasksController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITaskService _taskService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(
           ITaskService taskService,
           IEmployeeService employeeService,
           UserManager<ApplicationUser> userManager)
        {
            _taskService = taskService;
            _employeeService = employeeService;
            _userManager = userManager;
        }


        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            ViewBag.IsManager = false; // Default value

            try
            {
                //

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var applicationUser = await _userManager.FindByIdAsync(userId);

                // Find employee associated with current user
                var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                // var manager = await _employeeService.GetManagerEmployeeByUserIdAsync(userId);

                if (employee == null)
                {
                    return View(new List<TaskMB>());
                }

                var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(employee.Id);

                ViewBag.Employees = managedEmployees.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.FullName
                }).ToList();
                // If current user is a manager, get their employees
                if (User.IsInRole("Manager"))
                {


                    // Get tasks ASSIGNED BY this manager (for managers)
                    var tasksAssignedByManager = await _taskService.GetTasksByManagerAsync(employee.Id);
                    ViewBag.IsManager = true;
                    return View(tasksAssignedByManager);
                }
                else if (employee != null)
                {
                    // Get tasks assigned TO this employee (for regular employees)
                    var tasks = await _taskService.GetTasksByEmployeeAsync(employee.Id);


                    ViewBag.Employees = managedEmployees.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.FullName
                    }).ToList();

                    //tasks.CreatedByManagerName = EmployeeOfTask.ManagerName;
                    //CreatedByManagerName 




                    ViewBag.IsManager = false;
                    return View(tasks);
                }
                else
                {
                    ViewBag.IsManager = false;
                    return View(new List<TaskMB>());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Tasks/Create
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }
            var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);


            if (currentEmployee == null)
            {
                TempData["ErrorMessage"] = "Manager profile not found.";
                return RedirectToAction("Index");
            }

            // Get employees managed by current manager
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(currentEmployee.Id);

            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName");


            return View(new TaskMB());
        }

        // POST: Tasks/Create
        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskMB model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get current user and employee (manager)
                    var currentUser = await _userManager.GetUserAsync(User);
                    var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);

                    // Get assigned employee
                    var assignedEmployee = await _employeeService.GetEmployeeByIdAsync(model.EmployeeId);


                    //CreatedByManagerId
                    model.CreatedByManagerId = currentEmployee?.Id ?? 0;

                    var result = await _taskService.CreateTaskAsync(model);

                    if (result != null)
                    {
                        TempData["SuccessMessage"] = "Task created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to create task. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Exception", error = ex.Message });
                }
            }

            // Reload dropdown data if validation fails
            var reloadUser = await _userManager.GetUserAsync(User);
            var reloadEmployee = await _employeeService.GetEmployeeByUserIdAsync(reloadUser.Id);
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(reloadEmployee.Id);

            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName", model.EmployeeId);

            return PartialView("Create", model);
        }

        // GET: Tasks/MyTasks
        public async Task<IActionResult> MyTasks()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);

            if (currentEmployee == null)
            {
                TempData["ErrorMessage"] = "Employee profile not found.";
                return RedirectToAction("Index", "Home");
            }

            var tasks = await _taskService.GetTasksByEmployeeAsync(currentEmployee.Id);
            return View(tasks);
        }

        // POST: Tasks/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int taskId, TaskStatus newStatus)
        {
            try
            {
                var result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus);
                if (result)
                {
                    return Json(new { success = true, message = "Task status updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update task status." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userManager.GetUserAsync(User);
            var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(currentEmployee.Id);
            var EmployeeOfTask = await _employeeService.GetEmployeeByIdAsync(task.EmployeeId);

            var employee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);
            task.CreatedByManagerName = EmployeeOfTask.ManagerName;
            task.EmployeeName = EmployeeOfTask.FullName;

            if (employee == null)
            {
                return NotFound("Employee profile not found");
            }


            return PartialView("Details", task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentUser = await _userManager.GetUserAsync(User);
            var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(currentEmployee.Id);

            var employee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);

            if (employee == null)
            {
                return NotFound("Employee profile not found");
            }

            var EmployeeOfTask = await _employeeService.GetEmployeeByIdAsync(task.EmployeeId);
            task.CreatedByManagerName = EmployeeOfTask.ManagerName;

            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName", task.EmployeeId);
            ViewBag.StatusOptions = new SelectList(Enum.GetValues(typeof(TaskStatus)), task.Status);

            // Check if the user is a manager and return the appropriate view
            if (User.IsInRole("Manager"))
            {
                return PartialView("EditManager", task);
            }
            else
            {
                return PartialView("Edit", task);
            }

        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, TaskMB model)
        {

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var employee = await _employeeService.GetEmployeeByUserIdAsync(currentUserId);

                if (ModelState.IsValid)
                {


                    var result = await _taskService.UpdateTaskAsync(model);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Task updated successfully.";
                        return Json(new { success = true, message = "Task updated successfully." });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update task. Please try again.");
                    }
                }

            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }



            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new {
                    Key = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();

            return Json(new
            {
                success = false,
                message = "Invalid model state",
                validationErrors = errors,
                submittedData = new
                {
                    Id = model.Id,
                    Status = model.Status,
                    Title = model.Title,
                    EmployeeId = model.EmployeeId,

                }
            });
        }

        // POST: Tasks/UpdateAssignment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateAssignment(int id, TaskMB model)
        {
            if (id != model.Id)
            {
                return Json(new { success = false, message = "ID mismatch" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = await _employeeService.GetManagerEmployeeByUserIdAsync(userId);

            if (manager == null)
            {
                return Json(new { success = false, message = "Manager profile not found" });
            }

            // Verify that the task was created by this manager
            var task = await _taskService.GetTaskByIdAsync(id);

            //if (task == null || task.CreatedById != manager.Id)
            //{
            //    return Json(new { success = false, message = "You can only reassign tasks you created" });
            //}

            // Verify that the new employee is managed by this manager
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(manager.Id);
            if (!managedEmployees.Any(e => e.Id == model.EmployeeId))
            {
                return Json(new { success = false, message = "You can only assign tasks to employees you manage" });
            }

            if (ModelState.IsValid)
            {
                var updatedTask = await _taskService.UpdateTaskAssignmentAsync(id, model.EmployeeId);
                if (updatedTask == null)
                {
                    return Json(new { success = false, message = "Task not found" });
                }
                return Json(new { success = true, message = "Task reassigned successfully", task = updatedTask });
            }

            // Collect validation errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new {
                    Key = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();

            return Json(new
            {
                success = false,
                message = "Invalid model state",
                validationErrors = errors
            });
        }


        // GET: Tasks/AssignTaskModal/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignTask(int id)
        {
            var taskViewModel = await _taskService.GetTaskByIdAsync(id);
            if (taskViewModel == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var manager = await _employeeService.GetEmployeeByUserIdAsync(userId);

            if (manager == null)
            {
                return NotFound("Manager profile not found");
            }

            // Verify that the task was created by this manager
            //if (taskViewModel.CreatedById != manager.Id)
            //{
            //    return Forbid("You can only reassign tasks you created");
            //}

            // Get list of employees managed by this manager
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(manager.Id);
            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName");

            return PartialView("_AssignTask", taskViewModel);
        }




        // GET: Tasks/Delete/5 - إضافة method جديد
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int Id)
        {
            var task = await _taskService.GetTaskByIdAsync(Id);
            if (task == null)
            {
                return NotFound();
            }

            return PartialView("Delete", task);
        }

        // POST: Tasks/Delete/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Task deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete task." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

    }
}
