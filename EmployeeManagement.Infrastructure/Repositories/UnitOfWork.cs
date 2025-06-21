using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Departments = new DepartmentRepository(_dbContext);
            Employees = new EmployeeRepository(_dbContext);
            Tasks = new TaskRepository(_dbContext);
        }

        public IEmployeeRepository Employees { get; private set; }
        public IDepartmentRepository Departments { get; private set; }
        public ITaskRepository Tasks { get; private set; }

        public IEmployeeTaskRepository EmployeeTasks { get; private set; }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        // Not Now
        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync()
        {
            throw new NotImplementedException();
        }
        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }
    }
}
