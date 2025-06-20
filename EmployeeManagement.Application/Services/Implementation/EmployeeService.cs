using AutoMapper;
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
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync()
        {
            var employees = await _unitOfWork.Employees
               .GetAllAsync(includeProperties: "Department,Manager");

            return _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
        }

        public async Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(filter: e => e.Id == id, includeProperties: "Department,Manager");

            return _mapper.Map<EmployeeViewModel>(employee);
        }

        public async Task<EmployeeViewModel> CreateEmployeeAsync(EmployeeViewModel employeeViewModel)
        {
            var employee = _mapper.Map<Employee>(employeeViewModel);
            //var employee = new Employee
            //{
            //    FirstName = employeeViewModel.FirstName,
            //    LastName = employeeViewModel.LastName,
            //    Salary = employeeViewModel.Salary,
            //    DepartmentId = employeeViewModel.DepartmentId,
            //    ManagerId = employeeViewModel.ManagerId,
            //    ImagePath = employeeViewModel.ImagePath,
            //    CreatedAt = DateTime.UtcNow,
            //    UpdateDate = DateTime.UtcNow,
            //    IsActive = true
            //};


            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<EmployeeViewModel>(employee);
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

        


    }
}
