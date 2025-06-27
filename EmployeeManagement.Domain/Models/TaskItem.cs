using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Models
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public int EmployeeId { get; set; }
        public int? CreatedByManagerId { get; set; }

        // Navigation Properties
        public virtual Employee AssignedEmployee { get; set; }
        public virtual Employee CreatedByManager { get; set; }
        public virtual ICollection<EmployeeTask> EmployeeTasks { get; set; }


    }
    public enum TaskStatus
    {
        New,
        InProgress,
        Completed,
        Delayed,
        Cancelled
    }
}
