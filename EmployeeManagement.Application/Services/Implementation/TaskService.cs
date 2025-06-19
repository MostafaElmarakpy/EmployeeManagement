using AutoMapper;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.Services.Interfaces;
using EmployeeManagement.Application.ViewModels;
using EmployeeManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskViewModel>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskViewModel>>(tasks);

        }
        public async Task<TaskViewModel> CreateTaskAsync(TaskViewModel taskViewModel)
        {
            var task = _mapper.Map<TaskItem>(taskViewModel);
            task.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TaskViewModel>(task);
        }
        public async Task UpdateTaskAsync(TaskViewModel taskViewModel)
        {
            var task = _mapper.Map<TaskItem>(taskViewModel);
            task.UpdateDate = DateTime.UtcNow;

            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task != null)
            {
                _unitOfWork.Tasks.Delete(task);
                await _unitOfWork.SaveChangesAsync();
            }
        }



        public Task<IEnumerable<TaskViewModel>> GetOverdueTasksAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<TaskViewModel> GetTaskByIdAsync(int id)
        {
            var task = _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }
            return _mapper.Map<TaskViewModel>(task);
        }

        public async Task<IEnumerable<TaskViewModel>> GetTasksByEmployeeAsync(int employeeId)
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByManagerIdAsync(employeeId);
            if (tasks == null || !tasks.Any())
            {
                throw new KeyNotFoundException($"No tasks found for employee with ID {employeeId}.");
            }
            return _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
        }

        public async Task<IEnumerable<TaskViewModel>> GetTasksByStatusAsync(Domain.Models.TaskStatus status)
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByStatusAsync(status);
            return _mapper.Map<IEnumerable<TaskViewModel>>(tasks);
        }

        public async Task UnassignTaskFromEmployeeAsync(int taskId, int employeeId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");
            }

            // Check if the task is assigned to the specified employee
            var isAssignedToEmployee = task.EmployeeTasks.Any(et => et.EmployeeId == employeeId);
            if (!isAssignedToEmployee)
            {
                throw new InvalidOperationException($"Task with ID {taskId} is not assigned to employee with ID {employeeId}.");
            }

            // Remove the assignment
            var employeeTask = task.EmployeeTasks.FirstOrDefault(et => et.EmployeeId == employeeId);
            if (employeeTask != null)
            {
                task.EmployeeTasks.Remove(employeeTask);
            }

            task.UpdateDate = DateTime.UtcNow;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task AssignTaskToEmployeeAsync(int taskId, int employeeId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");
            }

            // Check if the task is already assigned to the specified employee
            var isAlreadyAssigned = task.EmployeeTasks.Any(et => et.EmployeeId == employeeId);
            if (isAlreadyAssigned)
            {
                throw new InvalidOperationException($"Task with ID {taskId} is already assigned to employee with ID {employeeId}.");
            }

            // Assign the task to the employee
            var employeeTask = new EmployeeTask
            {
                TaskId = taskId,
                EmployeeId = employeeId,
                AssignedDate = DateTime.UtcNow
            };
            task.EmployeeTasks.Add(employeeTask);

            task.UpdateDate = DateTime.UtcNow;
            _unitOfWork.Tasks.Update(task);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task<IEnumerable<TaskViewModel>> GetTasksByStatusAsync(System.Threading.Tasks.TaskStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
