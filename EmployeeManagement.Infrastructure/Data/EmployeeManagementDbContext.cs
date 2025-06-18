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
   public  class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public IConfiguration configuration { get; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> EmployeeTasks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options
            , IConfiguration configuration)
            : base(options)
        {
            this.configuration = configuration;
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



        }


      
        
    }
}

