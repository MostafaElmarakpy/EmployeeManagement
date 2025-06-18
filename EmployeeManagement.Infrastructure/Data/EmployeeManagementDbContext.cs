using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Http;


namespace EmployeeManagement.Infrastructure.Data
{
    class EmployeeManagementDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor accessor;
        public IConfiguration configuration { get; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> EmployeeTasks { get; set; }

        public EmployeeManagementDbContext(DbContextOptions<EmployeeManagementDbContext> options
            , IConfiguration configuration
            , IHttpContextAccessor accessor)
            : base(options)
        {
            this.configuration = configuration;
            this.accessor = accessor;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            // Configure Department-Employee relationship cannt delete Department if it has Employees

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configure self-referencing relationship for Employee (Manager-Managed Employees)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.ManagedEmployees)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Employee>()
                .HasMany(e => e.AssignedTasks)
                .WithOne(t => t.AssignedToEmployee)
                .HasForeignKey(t => t.AssignedToEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            DateTime dateNow = DateTime.Now;
            string userId = accessor!.HttpContext == null
                ? string.Empty
                : accessor!.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                           e.State == EntityState.Added
                        || e.State == EntityState.Modified
                        || e.State == EntityState.Deleted)
            );

            

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}

