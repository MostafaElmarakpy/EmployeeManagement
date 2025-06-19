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
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.FullName));


            // ViewModel to Domain
            CreateMap<DepartmentViewModel, Department>();
            CreateMap<EmployeeViewModel, Employee>();
            CreateMap<EmployeeViewModel, EmployeeViewModel>();
            // Custom mapping for EmployeeViewModel to Employee with ImageFile handling

            CreateMap<EmployeeViewModel, Employee>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
                .ForMember(dest => dest.ImageFile, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ManagedEmployees, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeTasks, opt => opt.Ignore());



        }
    }

}
