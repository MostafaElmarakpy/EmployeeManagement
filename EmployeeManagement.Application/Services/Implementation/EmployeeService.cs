using AutoMapper;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Application.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IUnitOfWork _unitOfWork;
         
        private readonly IMapper _mapper;
        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetAllEmployeesAsync()
        {
            var employees = await _unitOfWork.Employees
               .GetAllAsync(includeProperties: "Department,Manager");


            // maping Employee to EmployeeViewModel without AutoMapper
            var employeeViewModels = employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Salary = e.Salary,
                IsActive = e.IsActive,
                ImagePath = e.ImagePath,
                DepartmentId = e.DepartmentId,
                ManagerId = e.ManagerId,
                DepartmentName = e.Department?.Name,
                ManagerName = e.Manager?.FullName
            });
            return employeeViewModels;
            //return _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);


        }

        public async Task<EmployeeViewModel?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(filter: e => e.Id == id, includeProperties: "Department,Manager");

            return _mapper.Map<EmployeeViewModel>(employee);
        }

        public async Task<EmployeeViewModel> CreateEmployeeAsync(EmployeeViewModel employeeViewModel, IFormFile imageFile)
        {
            // Create employee 
     
            var employee = _mapper.Map<Employee>(employeeViewModel);

           
            employee.ImagePath = await SaveEmployeeImageAsync(imageFile);
            

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<EmployeeViewModel>(employee);
        }

        public async Task<EmployeeViewModel> UpdateEmployeeAsync(EmployeeViewModel employeeViewModel, IFormFile imageFile)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeViewModel.Id);

            if (employee == null)
                return null;

            var oldImagePath = employee.ImagePath;

            // If a new image is uploaded, handle image replacement first
            if (imageFile != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    DeleteImage(oldImagePath);
                }

                // Save new image and update ImagePath
                employee.ImagePath = await SaveEmployeeImageAsync(imageFile);
            }

            // Map other properties from view model to entity (excluding ImagePath if a new image was uploaded)
            _mapper.Map(employeeViewModel, employee);

            // Ensure ImagePath is not overwritten by the mapper if a new image was uploaded
            if (imageFile != null)
            {
                employee.ImagePath = employee.ImagePath; // keep the new image path
            }

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EmployeeViewModel>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(
                    filter: e => e.Id == id,
                    includeProperties: "ManagedEmployees"
                );

            if (employee == null)
                return false;

            // Check if employee is a manager
            if (employee.ManagedEmployees.Any())
                return false;

            // Delete employee image if it exists
            if (!string.IsNullOrEmpty(employee.ImagePath))
            {
                DeleteImage(employee.ImagePath);
            }
            _unitOfWork.Employees.Delete(employee);
                await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task AssignManagerAsync(int employeeId, int managerId)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee != null)
            {
                employee.ManagerId = managerId;
                employee.UpdateDate = DateTime.UtcNow;
                _unitOfWork.Employees.Update(employee);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<Employee?> GetEmployeeByUserIdAsync(string userId)
        {
            // Find employee where UserId matches the provided userId
            var employee = await _unitOfWork.Employees
                .GetFirstOrDefaultAsync(
                    filter: e => e.UserId == userId,
                    includeProperties: "Department,Manager"
                );

            return employee;
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetEmployeesByManagerAsync(int managerId)
        {
            var employees = await _unitOfWork.Employees
                .GetAllAsync(filter: e => e.ManagerId == managerId, includeProperties: "Department,Manager");
            return _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
        }

        public async Task<IEnumerable<EmployeeViewModel>> SearchEmployeesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllEmployeesAsync();
            }

            var employees = await _unitOfWork.Employees
                .GetAllAsync(
                    filter: e =>
                        (e.FirstName != null && e.FirstName.Contains(searchTerm)) ||
                        (e.LastName != null && e.LastName.Contains(searchTerm)) ||
                        (e.Department != null && e.Department.Name != null && e.Department.Name.Contains(searchTerm)),
                    includeProperties: "Department,Manager"
                );

            // Manual mapping to ensure DepartmentName and ManagerName are set
            var employeeViewModels = employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Salary = e.Salary,
                IsActive = e.IsActive,
                ImagePath = e.ImagePath,
                DepartmentId = e.DepartmentId,
                ManagerId = e.ManagerId,
                DepartmentName = e.Department?.Name,
                ManagerName = e.Manager?.FullName
            });

            return employeeViewModels;
        }

        public async Task<string> SaveEmployeeImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            // Validate file size (5MB limit)
            const long maxFileSize = 5 * 1024 * 1024; // 5MB
            if (image.Length > maxFileSize)
            {
                throw new InvalidOperationException("File size exceeds 5MB limit.");
            }

            // Validate file format
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Only .jpg, .png, and .gif files are allowed.");
            }

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generate unique filename
            var fileName = Guid.NewGuid() + fileExtension;
            var filePath = Path.Combine(uploadsPath, fileName);

            // Save file asynchronously
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Return relative URL path
            return $"/uploads/{fileName}";
        }

        private void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), imagePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

    }
}

