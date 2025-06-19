using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _unitOfWork.Employees.GetActiveEmployeesAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _unitOfWork.Employees.GetEmployeeWithDepartmentAsync(id);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            employee.IsActive = true;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return employee;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            employee.UpdateDate = DateTime.UtcNow;
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee != null)
            {
                employee.IsActive = false; // Soft delete
                employee.UpdateDate = DateTime.UtcNow;
                _unitOfWork.Employees.Update(employee);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await _unitOfWork.Employees.GetEmployeesByDepartmentAsync(departmentId);
        }

        public async Task AssignManagerAsync(int employeeId, int managerId)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee != null)
            {
                employee.ManagerId = managerId;
                employee.UpdateDate = DateTime.UtcNow;
                _unitOfWork.Employees.Update(employee);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
