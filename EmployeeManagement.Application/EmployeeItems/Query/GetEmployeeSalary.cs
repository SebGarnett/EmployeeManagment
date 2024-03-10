using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.EmployeeItems.Dtos;
using EmployeeManagement.Application.Interfaces;
using MediatR;

namespace EmployeeManagement.Application.EmployeeItems.Query
{
    public class GetEmployeeSalary : IRequest<EmployeeSalaryDto>
    {
        [Required]
        public Guid Id { get; set; }
        public double? Bonus { get; set; }
        public double? TaxDeduction { get; set; }
        public int? NbWorkedHours { get; set; }


    }

    public class GetEmployeeSalaryHandler : IRequestHandler<GetEmployeeSalary, EmployeeSalaryDto>
    {
        private readonly IAppDbContext _appDbContext;

        public GetEmployeeSalaryHandler(IAppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }
        public async Task<EmployeeSalaryDto> Handle(GetEmployeeSalary request, CancellationToken cancellationToken)
        {
            var emp = await _appDbContext.Employees.FindAsync(request.Id, cancellationToken);
            if (emp is null)
                throw new EmployeeNotFoundException("not found !");

            switch (emp.Type)
            {
                case 'F':
                    emp.BaseSalary = (emp.BaseSalary - request.TaxDeduction ?? 0) * 30 + request.Bonus ?? 0; // FullTime = base de 30 jours travaillés pour simplifier
                    return EmployeeSalaryDto.FromEmployeeEntity(emp);
                case 'C':
                    if (request.NbWorkedHours is null)
                        throw new ValidationException("NbWorkedHours should not be null for a consultant !");
                    if (request.NbWorkedHours <= 0)
                        throw new ValidationException("NbWorkedHours should be greater than 0 !");

                    emp.BaseSalary = (double)(emp.BaseSalary * request.NbWorkedHours)!; // Consultant donc calcul par le nb d'heures travaillées
                    return EmployeeSalaryDto.FromEmployeeEntity(emp);
                default:
                    throw new InvalidDataException($"There is no employee type : {emp.Type}");
            }
        }
    }
}
