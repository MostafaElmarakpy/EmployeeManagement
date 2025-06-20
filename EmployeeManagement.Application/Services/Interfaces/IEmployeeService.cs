﻿using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync();
        Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id);
        Task<EmployeeViewModel> GetEmployeeByUserIdAsync(string userId);
        Task<EmployeeViewModel> CreateEmployeeAsync(EmployeeViewModel employee);
        Task UpdateEmployeeAsync(EmployeeViewModel employee);
        Task DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<EmployeeViewModel>> GetEmployeesByManagerAsync(int managerId);
        Task<IEnumerable<EmployeeViewModel>> SearchEmployeesAsync(string searchTerm);
        Task AssignManagerAsync(int employeeId, int managerId);
    }
}
