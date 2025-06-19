using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }      
        public DateTime? DeleteDate { get; set; }   

        public bool IsActive { get; set; } = true;

    }
}
