using AutoMapper;
using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to ViewModel  
            CreateMap<Department, DepartmentViewModel>();

            // Map Employee to EmployeeViewModel with Department and Manager names and with ImageFile handling  
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath));

            // ViewModel to Domain  
            CreateMap<DepartmentViewModel, Department>();

            CreateMap<EmployeeViewModel, Employee>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
                .ForMember(dest => dest.ImageFile, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ManagedEmployees, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeTasks, opt => opt.Ignore());

            CreateMap<EmployeeViewModel, EmployeeViewModel>();

            // Task Mappings 
            CreateMap<TaskMB, TaskItem>()
                .ForMember(dest => dest.AssignedEmployee, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByManager, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeTasks, opt => opt.Ignore());

            // Map TaskItem to TaskMB

            CreateMap<TaskItem, TaskMB>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.AssignedEmployee != null ? src.AssignedEmployee.FullName : null))
                .ForMember(dest => dest.CreatedByManagerName, opt => opt.MapFrom(src => src.CreatedByManager != null ? src.CreatedByManager.FullName : null))
                .ForMember(dest => dest.AssignedDate, opt => opt.Ignore()); 
           
            

            CreateMap<EmployeeTask, TaskMB>()
                // Map core task properties from TaskItem
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Task.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Task.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Task.Description))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Task.Status))

                // Map employee information
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Employee.Id))

                // Map manager information correctly
                .ForMember(dest => dest.CreatedByManagerName,
                    opt => opt.MapFrom(src => src.Task.CreatedByManager.FullName));
        }
    }
}
