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
        Task<int> SaveChangesAsync();
        void BeginTransaction();
        Task CommitTransactionAsync();
        void RollbackTransaction();
    }
  
}
