using EmployeeManagement.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentViewModel>> GetAllDepartmentsAsync();
        Task<DepartmentViewModel> GetDepartmentByIdAsync(int id);
        Task<DepartmentViewModel> CreateDepartmentAsync(DepartmentViewModel departmentViewModel);
        Task UpdateDepartmentAsync(DepartmentViewModel departmentViewModel);
        Task DeleteDepartmentAsync(int id);
        Task<IEnumerable<DepartmentViewModel>> GetDepartmentsWithEmployeesAsync();
        //Task<bool> AssignManagerToDepartmentAsync(int departmentId, int managerId);
        Task<bool> CanDeleteDepartmentAsync(int id);
        Task<IEnumerable<DepartmentViewModel>> SearchDepartmentsAsync(string searchTerm);
    }
}
