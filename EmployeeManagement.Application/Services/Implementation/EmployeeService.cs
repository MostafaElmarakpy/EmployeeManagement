using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
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

        public async Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null || !employee.IsActive)
            {
                return null; // Employee not found or inactive
            }
            return new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Salary = employee.Salary,
                DepartmentId = employee.DepartmentId,
                ManagerId = employee.ManagerId,
                ImagePath = employee.ImagePath,
            };
        }

        public async Task<Employee> CreateEmployeeAsync(EmployeeViewModel employeeViewModel)
        {
            var employee = new Employee
            {
                FirstName = employeeViewModel.FirstName,
                LastName = employeeViewModel.LastName,
                Salary = employeeViewModel.Salary,
                DepartmentId = employeeViewModel.DepartmentId,
                ManagerId = employeeViewModel.ManagerId,
                ImagePath = employeeViewModel.ImagePath,
                CreatedAt = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployeeAsync(EmployeeViewModel employeeViewModel)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeViewModel.Id);
            if (employee != null)
            {
                employee.FirstName = employeeViewModel.FirstName;
                employee.LastName = employeeViewModel.LastName;
                employee.Salary = employeeViewModel.Salary;
                employee.DepartmentId = employeeViewModel.DepartmentId;
                employee.ManagerId = employeeViewModel.ManagerId;
                employee.ImagePath = employeeViewModel.ImagePath;
                employee.UpdateDate = DateTime.UtcNow;

                _unitOfWork.Employees.Update(employee);
                await _unitOfWork.SaveChangesAsync();
            }
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

        Task<EmployeeViewModel?> IEmployeeService.GetEmployeeByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


    }
}
