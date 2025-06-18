using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Models
{
    public class TaskAssignment : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int AssignedToEmployeeId { get; set; }
        public int ManagerId { get; set; }

        [ForeignKey(nameof(AssignedToEmployeeId))]
        public Employee AssignedToEmployee { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public Employee Manager { get; set; }

    }
}
