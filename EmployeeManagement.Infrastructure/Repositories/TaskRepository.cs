using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TaskRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

              

                 
        public  async Task<IEnumerable<TaskItem>> GetTasksByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.Tasks
                .Include(t => t.EmployeeTasks)
                .ThenInclude(te => te.Employee)
                .ToListAsync();

        }

        public  async Task<IEnumerable<TaskItem>> GetTasksByManagerIdAsync(int managerId)
        {
            return await _dbContext.Tasks
                .Include(t => t.EmployeeTasks)
                .ThenInclude(te => te.Employee)
                //.Where(t => t.EmployeeTasks.ManagerId == managerId)
                .ToListAsync();

        }
                 
        public  async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Domain.Models.TaskStatus status)
        {
            return await _dbContext.Tasks
                .Where(t => t.Status == status)
                .ToListAsync();
        }


    }
}
