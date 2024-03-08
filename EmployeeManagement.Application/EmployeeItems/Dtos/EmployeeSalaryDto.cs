using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.EmployeeItems.Dtos
{
    public class EmployeeSalaryDto
    {
        public Guid Id { get; set; }
        public double Salary { get; set; }

        public EmployeeSalaryDto(Guid id, double salary)
        {
            Id = id;
            Salary = salary;
        }

        public static EmployeeSalaryDto FromEmployeeEntity(Employee employee) =>
        new(employee.Id, employee.BaseSalary);

    }
}
