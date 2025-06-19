using EmployeeManagement.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.Interfaces
{
    public interface ITaskService 
    {
        Task<IEnumerable<TaskViewModel>> GetAllTasksAsync();
        Task<TaskViewModel> GetTaskByIdAsync(int id);
        Task<TaskViewModel> CreateTaskAsync(TaskViewModel taskViewModel);
        Task UpdateTaskAsync(TaskViewModel taskViewModel);
        Task DeleteTaskAsync(int id);
        Task<IEnumerable<TaskViewModel>> GetTasksByEmployeeAsync(int employeeId);
        Task<IEnumerable<TaskViewModel>> GetTasksByStatusAsync(TaskStatus status);
        Task<IEnumerable<TaskViewModel>> GetOverdueTasksAsync();
        Task AssignTaskToEmployeeAsync(int taskId, int employeeId);
        Task UnassignTaskFromEmployeeAsync(int taskId, int employeeId);

    }   
}
