using EmployeeManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
   public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync();
        Task<Department> GetDepartmentWithManagerAsync(int id);
        Task<IEnumerable<Department>> GetDepartmentsByBudgetRangeAsync(decimal minBudget, decimal maxBudget);
        Task<bool> DeleteDepartmentAsync(int id);

    }
}
