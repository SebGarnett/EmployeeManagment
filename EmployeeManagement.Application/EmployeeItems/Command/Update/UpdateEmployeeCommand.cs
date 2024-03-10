using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.EmployeeItems.Dtos;
using EmployeeManagement.Application.Interfaces;
using MediatR;

namespace EmployeeManagement.Application.EmployeeItems.Command.Update
{
    public class UpdateEmployeeCommand : IRequest<Unit>
    {
        [Required]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double? BaseSalary { get; set; }
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public UpdateEmployeeCommandHandler(IAppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }
        public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            #region Validation Controls

            if (request.BaseSalary is not null && request.BaseSalary <= 0)
                throw new ValidationException("Base salary should be greater than 0 !");

            if(request.Name is not null && request.Name.All(char.IsWhiteSpace))
                throw new ValidationException("Name should not be empty !");
            
            if(request.Name is not null && request.Name.Any(char.IsNumber))
                throw new ValidationException("The employee name should not have numbers !");

            #endregion

            if (!_appDbContext.Employees.Any(n => n.Id == request.Id))
                throw new EmployeeNotFoundException("Not found !");

            var emp = await _appDbContext.Employees.FindAsync(request.Id);

            if (request.BaseSalary is not null && request.BaseSalary > 0)
                emp!.BaseSalary = (double)request.BaseSalary;

            if(!string.IsNullOrEmpty(request.Name) || !string.IsNullOrWhiteSpace(request.Name))
                emp.Name = request.Name;
           
            

            _appDbContext.Employees.Update(emp);

            await _appDbContext.SaveChangesAsync(default);

            return Unit.Value;
        }
    }
}
