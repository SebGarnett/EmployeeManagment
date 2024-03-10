using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Interfaces;
using MediatR;

namespace EmployeeManagement.Application.EmployeeItems.Command.Delete
{
    public class DeleteEmployeeCommand : IRequest<Unit>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
    {
        private readonly IAppDbContext _dbContext;

        public DeleteEmployeeCommandHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (_dbContext.Employees.Any(n => n.Id == request.Id))
            {
                var emp = await _dbContext.Employees.FindAsync(request.Id);
                _ = _dbContext.Employees.Remove(emp);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }

            throw new EmployeeNotFoundException($"Employee {request.Id} not found in database !");

        }
    }
}
