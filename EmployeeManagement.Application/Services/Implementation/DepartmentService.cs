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

        public async Task UpdateDepartmentAsync(DepartmentViewModel departmentViewModel)
        {
            var department = _mapper.Map<Department>(departmentViewModel);
            department.UpdateDate = DateTime.UtcNow;

            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<IEnumerable<DepartmentViewModel>> GetAllDepartmentsAsync()
        {
            var departments = await _unitOfWork.Departments
                .GetAllAsync(includeProperties: "Employees");
            var departmentViewModels = departments.Select(d =>
            {
                var vm = _mapper.Map<DepartmentViewModel>(d);
                vm.EmployeeCount = d.Employees.Count;
                vm.TotalSalary = d.Employees.Sum(e => e.Salary);
                return vm;
            });

            return departmentViewModels;

        }

        public async Task<DepartmentViewModel> GetDepartmentByIdAsync(int id)
        {
 
            var department = await _unitOfWork.Departments
                .GetFirstOrDefaultAsync( filter :d => d.Id == id, includeProperties: "Employees");

            if (department == null)
                return null;

            var departmentViewModel = _mapper.Map<DepartmentViewModel>(department);
            departmentViewModel.EmployeeCount = department.Employees.Count;
            departmentViewModel.TotalSalary = department.Employees.Sum(e => e.Salary);

            return departmentViewModel;



            return _mapper.Map<DepartmentViewModel>(department);
        }

        public async Task<IEnumerable<DepartmentViewModel>> GetDepartmentsWithEmployeesAsync()
        {
            throw new NotImplementedException();
        }
        public async Task DeleteDepartmentAsync(int id)
        {
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
