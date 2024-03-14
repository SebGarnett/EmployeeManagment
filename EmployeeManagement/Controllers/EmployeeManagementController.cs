using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.EmployeeItems.Command.Create;
using EmployeeManagement.Application.EmployeeItems.Command.Delete;
using EmployeeManagement.Application.EmployeeItems.Command.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.EmployeeItems.Dtos;
using EmployeeManagement.Application.EmployeeItems.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeManagementController : ControllerBase
    {
        private ISender? _mediator;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        /// <summary>
        /// Creates an Employee from body's infos
        /// </summary>
        /// <param name="command"></param>
        /// <response code="200">Employee created successfully</response>
        [HttpPost("/employee/createEmployee")]
        [SwaggerResponse(200, "Employee created successfully", Type = typeof(EmployeeDto))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ObjectResult> Create(CreateEmployeeCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (ValidationException ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("/employee/calculateEmployeeSalary")]
        [SwaggerResponse(200,"Salary calculated successfully", Type= typeof(EmployeeSalaryDto))]
        public async Task<ObjectResult> CalculateEmployeeSalary([FromBody] GetEmployeeSalary command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound($"Employee with Id :{command.Id} not found");

            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("/employee/updateEmployee")]
        [SwaggerResponse(200, "Employee sucessfully updated")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeCommand command)
        {
            try
            {
                await Mediator.Send(command);
                return Ok();
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound($"Employee with Id :{command.Id} not found");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("/employee/deleteEmployee/{Id}")]
        [SwaggerResponse(200,"Employee successfully deleted")]
        public async Task<IActionResult> DeleteEmployee(Guid Id)
        {
            try
            {
                DeleteEmployeeCommand command = new DeleteEmployeeCommand
                {
                    Id = Id
                };
                await Mediator.Send(command);
                return Ok();
            }
            catch (EmployeeNotFoundException)
            {

                return NotFound($"Employee with Id :{Id} not found");
            }
        }

    }
}
