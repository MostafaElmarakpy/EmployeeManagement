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


            // Map Employee to EmployeeViewModel with Department and Manager names
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null))
                .AfterMap((src, dest) =>
                {
                    Console.WriteLine($"Mapping Employee: {src.Id} -> {dest.Id}, Department: {dest.DepartmentName}, Manager: {dest.ManagerName}");
                });


            // ViewModel to Domain  
            CreateMap<DepartmentViewModel, Department>();
            CreateMap<EmployeeViewModel, Employee>();
            CreateMap<EmployeeViewModel, EmployeeViewModel>();

            // Custom mapping for EmployeeViewModel to Employee with ImageFile handling  
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
                .AfterMap((src, dest) =>
                {
                    Console.WriteLine($"Mapping ImagePath: {src.ImagePath} -> {dest.ImagePath}");
                });
            // 
            CreateMap<TaskMB, TaskItem>();
            CreateMap<TaskItem, TaskMB>();
        }
    }

}
