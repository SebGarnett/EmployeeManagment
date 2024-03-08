using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }
        public char Type { get; set; }
        public string Name { get; set; }
        public double BaseSalary { get; set; }
    }
}
