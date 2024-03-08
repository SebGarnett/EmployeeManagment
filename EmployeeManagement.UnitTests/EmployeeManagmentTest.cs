using System;
using EmployeeManagement.Application.EmployeeItems.Command.Create;
using EmployeeManagement.Application.EmployeeItems.Command.Delete;
using EmployeeManagement.Application.EmployeeItems.Command.Update;
using EmployeeManagement.Application.EmployeeItems.Query;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Infrastructure;
using MediatR;

namespace EmployeeManagement.UnitTests
{
    public class EmployeeManagmentTest
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly CreateEmployeeCommandHandler _createEmployeeCommandHandler;
        private readonly GetEmployeeSalaryHandler _getEmployeeSalaryHandler;
        private readonly DeleteEmployeeCommandHandler _deleteEmployeeCommandHandler;
        private readonly UpdateEmployeeCommandHandler _updateEmployeeCommandHandler;


        public EmployeeManagmentTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: "EmployeeManagement_Test")
                .Options;
            _createEmployeeCommandHandler = new CreateEmployeeCommandHandler(new AppDbContext(_dbContextOptions));
            _getEmployeeSalaryHandler = new GetEmployeeSalaryHandler(new AppDbContext(_dbContextOptions));
            _deleteEmployeeCommandHandler = new DeleteEmployeeCommandHandler(new AppDbContext(_dbContextOptions));
            _updateEmployeeCommandHandler = new UpdateEmployeeCommandHandler(new AppDbContext(_dbContextOptions));
        }

        [Fact]
        public async Task CreateFullTimeEmployeeValid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 1000,
                StartDate = DateTime.Now
            };

            var result = _createEmployeeCommandHandler.Handle(request, default);


            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Result.Id);

            using (var dbContext = new AppDbContext(_dbContextOptions))
            {
                var createdEmployee = await dbContext.Employees!.FindAsync(result.Result.Id);
                Assert.NotNull(createdEmployee);

                var createdFullTimeEmployee = await dbContext.FullTimeEmployees.FindAsync(result.Result.Id);
                Assert.NotNull(createdFullTimeEmployee);
            }
        }

        [Fact]
        public async Task CreateConsultantVaid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 600,
                EndDate = DateTime.Now
            };

            var result = _createEmployeeCommandHandler.Handle(request, default);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Result.Id);

            using (var dbContext = new AppDbContext(_dbContextOptions))
            {
                var createdEmployee = await dbContext.Employees!.FindAsync(result.Result.Id);
                Assert.NotNull(createdEmployee);

                var createdConsultant = await dbContext.Consultants.FindAsync(result.Result.Id);
                Assert.NotNull(createdConsultant);
            }

        }

        [Fact]
        public Task GetFulTimeEmployeeMonthlySalaryValid()
        {
            var requestCreate = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 100,
                StartDate = DateTime.Now
            };

            var resultCreate = _createEmployeeCommandHandler.Handle(requestCreate, default);

            var request = new GetEmployeeSalary
            {
                Id = resultCreate.Result.Id,
                Bonus = 1000,
                TaxDeduction = 0,
                NbWorkedHours = null
            };

            var result = _getEmployeeSalaryHandler.Handle(request, default);
            Assert.NotNull(result);
            if (result.Result != null) 
                Assert.StrictEqual<int>(4000, (int)result.Result.Salary);

            return Task.CompletedTask;
        }
        [Fact]
        public Task GetConsultantEmployeeSalaryValid()
        {
            var requestCreate = new CreateEmployeeCommand
            {
                Name = "Jane Doe",
                BaseSalary = 75,
                EndDate = DateTime.Now.AddYears(1)
            };

            var resultCreate = _createEmployeeCommandHandler.Handle(requestCreate, default);

            var request = new GetEmployeeSalary
            {
                Id = resultCreate.Result.Id,
                Bonus = null,
                TaxDeduction = null,
                NbWorkedHours = 140
            };
            var result = _getEmployeeSalaryHandler.Handle(request, default);

            Assert.NotNull(result);
            if (result.Result != null)
                Assert.StrictEqual<int>(10500, (int)result.Result.Salary);

            return Task.CompletedTask;
        }

        [Fact]
        public Task DeleteEmployeeValid()
        {
            var requestCreate = new CreateEmployeeCommand
            {
                Name = "Jane Doe",
                BaseSalary = 75,
                EndDate = DateTime.Now.AddYears(1)
            };

            var resultCreate = _createEmployeeCommandHandler.Handle(requestCreate, default);

            var request = new DeleteEmployeeCommand
            {
                Id = resultCreate.Result.Id
            };

            var result = _deleteEmployeeCommandHandler.Handle(request, default);

            Assert.NotNull(result);
            Assert.StrictEqual(Unit.Value, result.Result);

            return Task.CompletedTask;
        }

        [Fact]
        public Task UpdateEmployeeValid()
        {
            var requestCreate = new CreateEmployeeCommand
            {
                Name = "Jane Doe",
                BaseSalary = 75,
                EndDate = DateTime.Now.AddYears(1)
            };

            var resultCreate = _createEmployeeCommandHandler.Handle(requestCreate, default);

            var request = new UpdateEmployeeCommand
            {
                Id = resultCreate.Result.Id,
                BaseSalary = 80,
            };

            var result = _updateEmployeeCommandHandler.Handle(request, default);

            Assert.NotNull(result);
            Assert.StrictEqual(Unit.Value, result.Result);

            var requestSalary = new GetEmployeeSalary
            {
                Id = resultCreate.Result.Id,
                NbWorkedHours = 140
            };

            var resultSalary = _getEmployeeSalaryHandler.Handle(requestSalary, default);

            Assert.NotNull(resultSalary);

            Assert.StrictEqual(11200, resultSalary.Result.Salary);

            return Task.CompletedTask;
        }
    }
}
