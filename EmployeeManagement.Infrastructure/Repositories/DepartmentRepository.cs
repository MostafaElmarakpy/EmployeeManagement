using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _dbContext.Departments.FindAsync(id);
            if (department == null)
            {
                return false; 

            }
            _dbContext.Departments.Remove(department);
            await _dbContext.SaveChangesAsync();
            return true; 
        }

        public Task<IEnumerable<Department>> GetDepartmentsByBudgetRangeAsync(decimal minBudget, decimal maxBudget)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync()
        {
            return await _dbContext.Departments.Include(d => d.Employees).ToListAsync();
        }

        public async Task<Department> GetDepartmentWithManagerAsync(int id)
        {
            return await _dbContext.Departments
                .Include(d => d.Manager) 
                .FirstOrDefaultAsync(d => d.Id == id);

        }

        public async Task<IEnumerable<Department>> GetDepartmentsWithDetailsAsync(string searchString)
        {
            var query = _dbContext.Departments
                .Include(d => d.Employees)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(d => d.Name.Contains(searchString));
            }

            return await query.ToListAsync();
        }
    }
}
