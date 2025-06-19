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
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> GetEmployeeByUserIdAsync(string userId);
        Task<Employee> GetManagerEmployeeByUserIdAsync(string userId);
        Task<IEnumerable<Employee>> GetEmployeesByManagerIdAsync(int managerId);

        Task<Employee> CreateEmployeeAsync(Employee employee, IFormFile imageFile);
        Task<Employee> UpdateEmployeeAsync(Employee employee, IFormFile imageFile);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> EmployeeIsManagerAsync(int id);
        Task<bool> EmployeeExistsAsync(int id);



    }
}
