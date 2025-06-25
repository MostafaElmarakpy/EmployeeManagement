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

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var applicationUser = await _userManager.FindByIdAsync(userId);

                // Find employee associated with current user
                var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                // var manager = await _employeeService.GetManagerEmployeeByUserIdAsync(userId);

                if (employee == null)
                {
                    return View(new List<TaskMB>());
                }

                // If current user is a manager, get their employees
                if (User.IsInRole("Manager"))
                {

                    var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(employee.Id);
                    ViewBag.Employees = managedEmployees.Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.FullName
                    }).ToList();

                    // Get tasks ASSIGNED BY this manager (for managers)
                    var tasksAssignedByManager = await _taskService.GetTasksByManagerAsync(employee.Id);
                    ViewBag.IsManager = true;
                    return View(tasksAssignedByManager);
                }
                else if (employee != null)
                {
                    // Get tasks assigned TO this employee (for regular employees)
                    var tasks = await _taskService.GetTasksByEmployeeAsync(employee.Id);
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


            //ViewBag.IsManager = isManager;
            //return View(new List<TaskMB>());
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var applicationUser = await _userManager.FindByIdAsync(userId);

            //Find employee associated with current user
            //var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);

            //var currentUser = await _userManager.GetUserAsync(User);
            //var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);

            //if (currentEmployee == null)
            //{
            //    //TempData["ErrorMessage"] = "Employee profile not found.";
            //    return View(new List<TaskMB>());
            //    //return RedirectToAction("Index", "Home");
            //}

            // If user is a manager, show tasks they created
            // If user is an employee, show tasks assigned to them
            //var isManager = User.IsInRole("Manager") || User.IsInRole("Admin");

            //IEnumerable<TaskMB> tasks;
            //if (isManager)
            //{
            //    tasks = await _taskService.GetTasksByManagerAsync(currentEmployee.Id);
            //}
            //else
            //{
            //    tasks = await _taskService.GetTasksByEmployeeAsync(currentEmployee.Id);
            //}

            //ViewBag.IsManager = isManager;
            //return View(tasks);
        }

        // GET: Tasks/Create
        [Authorize(Roles = "Manager,Admin")]
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
            ViewBag.Priorities = new SelectList(Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>()
                .Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text");

            var model = new TaskMB
            {
                CreatedByManagerId = currentEmployee.Id,
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7),
                Status = TaskStatus.New,
                Priority = TaskPriority.Medium
            };

            return View(model);
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Admin")]
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

                    // Set manager and employee names for the model
                    model.CreatedByManagerName = currentEmployee?.FullName;
                    model.EmployeeName = assignedEmployee?.FullName;
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
            ViewBag.Priorities = new SelectList(Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>()
                .Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text", (int)model.Priority);

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

            return PartialView("Details", task);
        }

        // GET: Tasks/Edit/5
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(currentEmployee.Id);

            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName", task.EmployeeId);
            ViewBag.Priorities = new SelectList(Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>()
                .Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text", (int)task.Priority);
            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>()
                .Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text", (int)task.Status);

            return PartialView("Edit", task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Edit(int id, TaskMB model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _taskService.UpdateTaskAsync(model);
                    if (result)
                    {
                        TempData["SuccessMessage"] = "Task updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update task. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating task: {ex.Message}");
                }
            }

            // Reload dropdown data if validation fails
            var currentUser = await _userManager.GetUserAsync(User);
            var currentEmployee = await _employeeService.GetEmployeeByUserIdAsync(currentUser.Id);
            var managedEmployees = await _employeeService.GetEmployeesByManagerAsync(currentEmployee.Id);

            ViewBag.Employees = new SelectList(managedEmployees, "Id", "FullName", model.EmployeeId);
            ViewBag.Priorities = new SelectList(Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>()
                .Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text", (int)model.Priority);
            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>()
                .Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text", (int)model.Status);

            return View(model);
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
    

        // POST: Tasks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Delete(int id)
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

