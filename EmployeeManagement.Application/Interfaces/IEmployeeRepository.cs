using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<Employee> GetEmployeeWithDepartmentAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesWithManagerAsync();
        Task<IEnumerable<Employee>> GetSubordinatesAsync(int managerId);
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();

    }
}
