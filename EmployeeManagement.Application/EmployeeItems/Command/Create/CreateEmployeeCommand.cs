using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.EmployeeItems.Dtos;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using MediatR;

namespace EmployeeManagement.Application.EmployeeItems.Command.Create
{
    public class CreateEmployeeCommand : IRequest<EmployeeDto>
    {
        [Required] public string Name { get; set; }
        [Required] public double BaseSalary { get; set; }
         public DateTime? StartDate { get; set; }
         public DateTime? EndDate { get;set; }
    }

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
    {
        private readonly IAppDbContext _dbContext;
        public CreateEmployeeCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            EmployeeDto empDto = null;
            if (request.StartDate is null)
            {
                //Consultant
                Consultant cons = new Consultant
                {
                    Id = new Guid(),
                    Type = 'C',
                    Name = request.Name,
                    BaseSalary = request.BaseSalary,
                    EndDate = request.EndDate
                };
                _dbContext.Consultants.Add(cons);
                empDto = EmployeeDto.FromConsultantEntity(cons);
            }
            else
            {
                //Full Time
                FullTimeEmployee emp = new FullTimeEmployee
                {
                    Id = new Guid(),
                    Type = 'F',
                    Name = request.Name,
                    BaseSalary = request.BaseSalary,
                    BeginDate = request.StartDate
                };
                _dbContext.FullTimeEmployees.Add(emp);
                empDto = EmployeeDto.FromFullTimeEmployeeEntity(emp);

            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            //todo controler les valeurs dans la request pour la sécurité
            return empDto;
        }
    }
}
