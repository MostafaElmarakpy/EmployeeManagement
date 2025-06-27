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

        public async Task<IEnumerable<TaskMB>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.Tasks.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskMB>>(tasks);
        }

        public async Task<TaskMB> GetTaskByIdAsync(int id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                return null;
            }
            return _mapper.Map<TaskMB>(task);
        }

        public async Task<TaskMB> CreateTaskAsync(TaskMB taskViewModel)
        {
            // Check if employee exists
            var employee = await _unitOfWork.Employees.GetByIdAsync(taskViewModel.EmployeeId);
            if (employee == null)
                throw new ArgumentException($"Employee with ID {taskViewModel.EmployeeId} does not exist.", nameof(taskViewModel.EmployeeId));

            await _unitOfWork.BeginTransaction();
            try
            {
                var task = _mapper.Map<TaskItem>(taskViewModel);
                //task.Status = Domain.Models.TaskStatus.New;
                task.CreatedByManagerId = taskViewModel.CreatedByManagerId;
                task.EmployeeId = taskViewModel.EmployeeId;//

                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveChangesAsync();

                var employeeTask = new EmployeeTask
                {
                    TaskId = task.Id,
                    EmployeeId = taskViewModel.EmployeeId,
                    AssignedDate = DateTime.UtcNow,
                    Employee = employee // Ensure navigation property is set if needed
                };

                await _unitOfWork.EmployeeTasks.AddAsync(employeeTask);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Set the EmployeeName in the returned view model
                var result = _mapper.Map<TaskMB>(task);
                result.EmployeeName = employee.FullName;
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<bool> UpdateTaskAsync(TaskMB taskViewModel)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskViewModel.Id);
                if (task == null)
                {
                    return false;
                }

                _mapper.Map(taskViewModel, task);
                task.UpdateDate = DateTime.UtcNow;

                // Ensure the EmployeeId is not changed
                task.EmployeeId = taskViewModel.EmployeeId;


                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(id);
                if (task == null)
                {
                    return false;
                }

                // Delete related EmployeeTasks first
                var employeeTasks = await _unitOfWork.EmployeeTasks.GetTaskAssignmentsByTaskIdAsync(id);
                foreach (var et in employeeTasks)
                {
                    _unitOfWork.EmployeeTasks.Delete(et);
                }

                _unitOfWork.Tasks.Delete(task);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"DeleteTaskAsync Error: {ex}");
                throw; 
            }
        }
        public async Task<IEnumerable<TaskMB>> GetTasksByEmployeeAsync(int employeeId)
        {
            var employeeTasks = await _unitOfWork.EmployeeTasks.GetTaskAssignmentsByEmployeeIdAsync(employeeId);
            var taskIds = employeeTasks.Select(et => et.TaskId).ToList();

            var tasks = await _unitOfWork.Tasks.GetTasksByIdsAsync(taskIds);
            return _mapper.Map<IEnumerable<TaskMB>>(tasks);
        }

        public async Task<IEnumerable<TaskMB>> GetTasksByManagerAsync(int managerId)
        {
            //var tasks = await _unitOfWork.EmployeeTasks.GetAllAsync(
            //        filter: t => t.CreatedById == managerId,
            //        includeProperties: "Employee"
            //    );
            var tasks = await _unitOfWork.Tasks.GetTasksByManagerIdAsync(managerId);
            return _mapper.Map<IEnumerable<TaskMB>>(tasks);
        }
        
        public async Task<IEnumerable<TaskMB>> GetTasksByStatusAsync(Domain.Models.TaskStatus status)
        {
            var tasks = await _unitOfWork.Tasks.GetTasksByStatusAsync(status);
            return _mapper.Map<IEnumerable<TaskMB>>(tasks);
        }

        public async Task<IEnumerable<TaskMB>> GetOverdueTasksAsync()
        {
            var tasks = await _unitOfWork.Tasks.GetOverdueTasksAsync();
            return _mapper.Map<IEnumerable<TaskMB>>(tasks);
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, Domain.Models.TaskStatus newStatus)
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null)
                {
                    return false;
                }

                task.Status = newStatus;
                task.UpdateDate = DateTime.UtcNow;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task AssignTaskToEmployeeAsync(int taskId, int employeeId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");
            }

            // Check if the task is already assigned to the specified employee
            var existingAssignment = await _unitOfWork.EmployeeTasks.GetTaskAssignmentAsync(taskId, employeeId);
            if (existingAssignment != null)
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

            await _unitOfWork.EmployeeTasks.AddAsync(employeeTask);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnassignTaskFromEmployeeAsync(int taskId, int employeeId)
        {
            var employeeTask = await _unitOfWork.EmployeeTasks.GetTaskAssignmentAsync(taskId, employeeId);
            if (employeeTask == null)
            {
                throw new KeyNotFoundException($"Task assignment not found for task {taskId} and employee {employeeId}.");
            }

            _unitOfWork.EmployeeTasks.Delete(employeeTask);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TaskMB> UpdateTaskAssignmentAsync(int taskId, int newEmployeeId)
        {
            var task = await _unitOfWork.EmployeeTasks
                .GetFirstOrDefaultAsync(
                    filter: t => t.TaskId == taskId,
                    includeProperties: "Employee"
                    //includeProperties: "Employee,CreatedByManager"
                );

            if (task == null)
                return null;

            task.EmployeeId = newEmployeeId;

            _unitOfWork.EmployeeTasks.Update(task);
            await _unitOfWork.SaveChangesAsync();

            // Refresh the task with updated employee info
            task = await _unitOfWork.EmployeeTasks
                .GetFirstOrDefaultAsync(
                    filter: t => t.TaskId == taskId,
                    includeProperties: "Employee"
                    //includeProperties: "Employee,CreatedBy"
                );

            return _mapper.Map<TaskMB>(task);
        }

    }
}

