using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IDepartmentRepository Departments { get; }
        ITaskRepository Tasks { get; }
        IEmployeeTaskRepository EmployeeTasks { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransaction();
        Task CommitTransactionAsync();
        Task RollbackTransaction();
    }
  
}
