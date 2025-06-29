using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";




        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }



        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }


        [Display(Name = "Profile Image")]
        public string ImagePath { get; set; }

        // For image upload - not mapped to database
        public IFormFile ImageFile { get; set; }


        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Manager")]
        public int? ManagerId { get; set; }

        // Navigation Properties for Display
        public string? DepartmentName { get; set; }
        public string? ManagerName { get; set; }

        //public IEnumerable<SelectListItem> Departments { get; set; }
        //public IEnumerable<SelectListItem> Managers { get; set; }
    }
}
