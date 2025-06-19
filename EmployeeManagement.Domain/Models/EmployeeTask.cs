using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Models
{
    public class EmployeeTask 
    {
        public int TaskId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AssignedDate { get; set; }

        public TaskItem Task { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; }

    }
}
