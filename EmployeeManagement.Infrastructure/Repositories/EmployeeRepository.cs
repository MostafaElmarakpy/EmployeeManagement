using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await _dbContext.Employees
                .Where(e => e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _dbContext.Employees
                .Where(e => e.DepartmentId == departmentId)
                .Include(e => e.Department) 
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithManagerAsync()
        {
            return await _dbContext.Employees
                .Include(e => e.Manager)
                .Where(e => e.ManagerId != null) 
                .ToListAsync();
        }

        public async Task<Employee> GetEmployeeWithDepartmentAsync(int id)
        {
            return await _dbContext.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetSubordinatesAsync(int managerId)
        {
            return await _dbContext.Employees
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();
        }

    }
}
