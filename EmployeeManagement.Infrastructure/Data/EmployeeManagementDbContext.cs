using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EmployeeManagement.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<EmployeeTask> EmployeeTasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            
            // Employee-Department relationship (Many-to-One)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee-Manager self-referencing relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(m => m.ManagedEmployees)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                 .Property(e => e.Salary)
                 .HasColumnType("decimal(18,2)");


            // Department-Manager relationship (One-to-One)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithOne()
                .HasForeignKey<Department>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Task-Employee Many-to-Many relationship
            modelBuilder.Entity<EmployeeTask>()
                .HasKey(te => new { te.TaskId, te.EmployeeId });

            modelBuilder.Entity<EmployeeTask>()
                .HasOne(te => te.Task)
                .WithMany(t => t.EmployeeTasks)
                .HasForeignKey(te => te.TaskId);

            modelBuilder.Entity<EmployeeTask>()
                .HasOne(te => te.Employee)
                .WithMany(e => e.EmployeeTasks)
                .HasForeignKey(te => te.EmployeeId);


            // TaskItem-Employee self-referencing relationship

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.AssignedEmployee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.CreatedByManager)
                .WithMany()
                .HasForeignKey(t => t.CreatedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);



        }




    }
}

