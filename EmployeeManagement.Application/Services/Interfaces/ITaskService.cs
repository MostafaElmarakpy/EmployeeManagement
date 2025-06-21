using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = EmployeeManagement.Domain.Models.TaskStatus;

namespace EmployeeManagement.Application.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskMB>> GetAllTasksAsync();
        Task<TaskMB> GetTaskByIdAsync(int id);
        Task<TaskMB> CreateTaskAsync(TaskMB taskViewModel);
        Task<bool> UpdateTaskAsync(TaskMB taskViewModel);
        Task<bool> DeleteTaskAsync(int id);
        Task<IEnumerable<TaskMB>> GetTasksByEmployeeAsync(int employeeId);
        Task<IEnumerable<TaskMB>> GetTasksByManagerAsync(int managerId);
        Task<IEnumerable<TaskMB>> GetTasksByStatusAsync(TaskStatus status);
        Task<IEnumerable<TaskMB>> GetOverdueTasksAsync();
        Task<bool> UpdateTaskStatusAsync(int taskId, TaskStatus newStatus);
        Task AssignTaskToEmployeeAsync(int taskId, int employeeId);
        Task UnassignTaskFromEmployeeAsync(int taskId, int employeeId);
    }
}

