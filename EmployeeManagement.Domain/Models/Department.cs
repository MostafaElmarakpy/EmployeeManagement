using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Models
{
    public class Department : BaseEntity
    {
       
        public string Name { get; set; }
        public string Description { get; set; }

        public int? ManagerId { get; set; }

        // Self-referencing relationship for the manager of the department
        public Employee Manager { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    
    
    }
}
