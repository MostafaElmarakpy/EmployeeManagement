using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Models
{
    public class Department : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
 
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    
    
    }
}
