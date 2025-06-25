using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services.Implementation;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class EmployeeTaskRepository : GenericRepository<EmployeeTask>, IEmployeeTaskRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EmployeeTaskRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<EmployeeTask>> GetTaskAssignmentsByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.EmployeeTasks
                .Where(et => et.EmployeeId == employeeId)
                .Include(et => et.Task)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeTask>> GetTaskAssignmentsByTaskIdAsync(int taskId)
        {
            return await _dbContext.EmployeeTasks
                .Where(et => et.TaskId == taskId)
                .Include(et => et.Employee)
                .ToListAsync();
        }

        public async Task<EmployeeTask> GetTaskAssignmentAsync(int taskId, int employeeId)
        {
            return await _dbContext.EmployeeTasks
                .FirstOrDefaultAsync(et => et.TaskId == taskId && et.EmployeeId == employeeId);
        }
    }
}