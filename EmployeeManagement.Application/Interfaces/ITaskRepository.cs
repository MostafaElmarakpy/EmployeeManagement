using EmployeeManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface ITaskRepository : IGenericRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Domain.Models.TaskStatus status);
        Task<IEnumerable<TaskItem>> GetTasksByManagerIdAsync(int managerId);



    }
}
