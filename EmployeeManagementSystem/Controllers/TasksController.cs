using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class TasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
