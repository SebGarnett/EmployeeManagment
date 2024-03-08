using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Entities;
using Microsoft.Extensions.Primitives;

namespace EmployeeManagement.Application.EmployeeItems.Dtos
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double BaseSalary { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public EmployeeDto(Guid id, string name, double baseSalary, DateTime? startDate, DateTime? endDate)
        {
            Id = id;
            Name = name;
            BaseSalary = baseSalary;
            StartDate = startDate;
            EndDate = endDate;
        }

        public static EmployeeDto FromFullTimeEmployeeEntity(FullTimeEmployee employee) =>
            new(employee.Id, employee.Name, employee.BaseSalary, employee.BeginDate, null);

        public static EmployeeDto FromConsultantEntity(Consultant consultant) =>
            new(consultant.Id, consultant.Name, consultant.BaseSalary, null, consultant.EndDate);
    }
}
