using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
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

       


        public async Task<IActionResult> Create()
        {
            return View();
        }
    }
}
