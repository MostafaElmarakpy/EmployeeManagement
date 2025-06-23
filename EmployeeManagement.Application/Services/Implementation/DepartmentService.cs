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
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<DepartmentViewModel> CreateDepartmentAsync(DepartmentViewModel departmentViewModel)
        {
            var department = _mapper.Map<Department>(departmentViewModel);
            department.CreatedAt = DateTime.UtcNow;
            department.UpdateDate = DateTime.UtcNow;
            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<DepartmentViewModel>(department);
        }

        public async Task<DepartmentViewModel> UpdateDepartmentAsync(DepartmentViewModel departmentViewModel)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentViewModel.Id);

            if (department == null)
                return null;

            _mapper.Map(departmentViewModel, department);

            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentViewModel>(department);

        }

        public async Task<IEnumerable<DepartmentViewModel>> GetAllDepartmentsAsync()
        {
            // Pseudocode for debugging EmployeeCount and TotalSalary calculation

            // 1. Fetch all departments with their employees included
            var departments = await _unitOfWork.Departments.GetAllAsync(includeProperties: "Employees");
            // 2. For each department, map to DepartmentViewModel
            var departmentViewModels = departments.Select(d =>
            {
                // 3. Map department entity to view model
                var vm = _mapper.Map<DepartmentViewModel>(d);

                // 4. Calculate EmployeeCount and TotalSalary
                //    - Check if d.Employees is not null
                //    - If null, set EmployeeCount = 0, TotalSalary = 0
                //    - If not null, set EmployeeCount = d.Employees.Count, TotalSalary = d.Employees.Sum(e => e.Salary)
                if (d.Employees != null)
                {
                    vm.EmployeeCount = d.Employees.Count;
                    vm.TotalSalary = d.Employees.Sum(e => e.Salary);
                }
                else
                {
                    vm.EmployeeCount = 0;
                    vm.TotalSalary = 0;
                }
                return vm;
            });
                
            // 5. Return the list of DepartmentViewModel
            return departmentViewModels;

        }

        public async Task<DepartmentViewModel> GetDepartmentByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments
                .GetFirstOrDefaultAsync(filter: d => d.Id == id, includeProperties: "Employees");

            if (department == null)
                return null;

            var departmentViewModel = _mapper.Map<DepartmentViewModel>(department);
            departmentViewModel.EmployeeCount = department.Employees.Count;
            departmentViewModel.TotalSalary = department.Employees.Sum(e => e.Salary);

            return departmentViewModel;
        }

        public async Task<bool> CanDeleteDepartmentAsync(int id)
        {
            var department = await _unitOfWork.Departments
                .GetFirstOrDefaultAsync(filter: d => d.Id == id, includeProperties: "Employees");

            // Check if there are no employees in the department
            return department?.Employees?.Count == 0; 
        }

        public async Task<IEnumerable<DepartmentViewModel>> SearchDepartmentsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllDepartmentsAsync();
            }

            var departments = await _unitOfWork.Departments
                .GetAllAsync(filter: d => d.Name.Contains(searchTerm), includeProperties: "Employees");

            var departmentViewModels = departments.Select(d =>
            {
                var vm = _mapper.Map<DepartmentViewModel>(d);
                vm.EmployeeCount = d.Employees.Count;
                vm.TotalSalary = d.Employees.Sum(e => e.Salary);
                return vm;
            });

            return departmentViewModels;
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            // Check if department can be deleted (no employees assigned)
            var canDelete = await CanDeleteDepartmentAsync(id);
            if (!canDelete)
            {
                throw new InvalidOperationException("Cannot delete department with assigned employees.");
            }

            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            _unitOfWork.Departments.Delete(department);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
