using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    public class FullTimeEmployee: Employee
    {
        public DateTime? BeginDate { get; set; }
    }
}
