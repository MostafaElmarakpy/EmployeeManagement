using EmployeeManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = EmployeeManagement.Domain.Models.TaskStatus;

namespace EmployeeManagement.Application.ViewModels
{
    public class TaskMB
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Task Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Task Title")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Due Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public TaskStatus Status { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public TaskPriority Priority { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Employee is required")]
        [Display(Name = "Assigned Employee")]
        public int EmployeeId { get; set; }

        [Display(Name = "Created By Manager")]
        public int CreatedByManagerId { get; set; }

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        // Navigation Properties for Display
        [Display(Name = "Employee Name")]
        public string? EmployeeName { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedByManagerName { get; set; }

        [Display(Name = "Department")]
        public string? DepartmentName { get; set; }

        // Computed Properties
        public bool IsOverdue => DueDate < DateTime.Now && Status != TaskStatus.Completed;
        public int DaysRemaining => (DueDate - DateTime.Now).Days;
        public string StatusDisplay => Status.ToString();
        public string PriorityDisplay => Priority.ToString();
    }

    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}

