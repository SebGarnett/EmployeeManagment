using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EmployeeManagement.API.Models;
using EmployeeManagement.Application.EmployeeItems.Command.Create;
using EmployeeManagement.Application.EmployeeItems.Command.Update;
using EmployeeManagement.Application.EmployeeItems.Dtos;
using EmployeeManagement.Application.EmployeeItems.Query;
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.IntegrationTests
{
    public class EmployeeManagementControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly WebApplicationFactory<Program> _app;

        public EmployeeManagementControllerTests(WebApplicationFactory<Program> app)
        {
            _app = app;
            _httpClient = _app.CreateClient();
            var token = ApiLogin(new LoginModel { Username = "Test", Password = "TestPassword" });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private string ApiLogin(LoginModel loginModel)
        {
            var resultLogin =  _httpClient.PostAsJsonAsync("/auth/login", loginModel).Result;
            resultLogin.EnsureSuccessStatusCode();

            var loginResponse =  resultLogin.Content.ReadFromJsonAsync<Login>().Result;
            return loginResponse!.token;
        }


        [Fact]
        public async Task CreateFullTimeEmployeeValid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 2000,
                StartDate = DateTime.Now
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeResponse);
            Assert.IsType<Guid>(employeeResponse.Id);

            _ = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{employeeResponse.Id}");
        }

        [Fact]
        public async Task CreateConsultantEmployeeValid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 2000,
                EndDate = DateTime.Now.AddMonths(10)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeResponse);
            Assert.IsType<Guid>(employeeResponse.Id);

            _ = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{employeeResponse.Id}");
        }

        [Fact]
        public async Task CreateEmployeeInvalidName()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Do3",
                BaseSalary = 2000,
                EndDate = DateTime.Now.AddMonths(9)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);
            Assert.StrictEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task CreateEmployeeInvalidSalary()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = -100,
                EndDate = DateTime.Now.AddMonths(9)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);
            Assert.StrictEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task CalculateFullTimeEmployeeSalaryValid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 2000,
                StartDate = DateTime.Now
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(result);

            var requestCalc = new GetEmployeeSalary
            {
                Id = employeeResponse!.Id,
                Bonus = 1000,
                TaxDeduction = 350
            };

            var resultCalc = await _httpClient.PostAsJsonAsync("/employee/calculateEmployeeSalary", requestCalc);
            resultCalc.EnsureSuccessStatusCode();

            var calcResponse = await resultCalc.Content.ReadFromJsonAsync<EmployeeSalaryDto>();
            Assert.NotNull(calcResponse);
            Assert.StrictEqual(50500, calcResponse.Salary);


            _ = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{calcResponse.Id}");
        }

        [Fact]
        public async Task CalculateEmployeeSalaryNotFound()
        {
            var requestCalc = new GetEmployeeSalary
            {
                Id = new Guid(),
                Bonus = 1000,
                TaxDeduction = 350
            };

            var resultCalc = await _httpClient.PostAsJsonAsync("/employee/calculateEmployeeSalary", requestCalc);

            Assert.StrictEqual(HttpStatusCode.NotFound, resultCalc.StatusCode);


        }

        [Fact]
        public async Task CalculateConsultantSalaryValid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 85,
                EndDate = DateTime.Now.AddMonths(10)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeResponse);


            var requestCalc = new GetEmployeeSalary
            {
                Id = employeeResponse.Id,
                NbWorkedHours = 140
            };

            var resultCalc = await _httpClient.PostAsJsonAsync("/employee/calculateEmployeeSalary", requestCalc);
            resultCalc.EnsureSuccessStatusCode();

            var calcResponse = await resultCalc.Content.ReadFromJsonAsync<EmployeeSalaryDto>();
            Assert.NotNull(calcResponse);
            Assert.StrictEqual(11900, calcResponse.Salary);


            _ = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{calcResponse.Id}");
        }

        [Fact]
        public async Task UpdateEmployeeValid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 85,
                EndDate = DateTime.Now.AddMonths(10)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeResponse);

            var requestUpdate = new UpdateEmployeeCommand
            {
                Id = employeeResponse.Id,
                Name = "Jane Doe"
            };

            var resultUpdate = await _httpClient.PutAsJsonAsync("/employee/updateEmployee", requestUpdate);
            Assert.StrictEqual(HttpStatusCode.OK,resultUpdate.StatusCode);

            _ = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{employeeResponse.Id}");
        }

        [Fact]
        public async Task UpdateEmployeeNotFound()
        {
            var requestUpdate = new UpdateEmployeeCommand
            {
                Id = new Guid(),
                Name = "Jane Doe"
            };

            var resultUpdate = await _httpClient.PutAsJsonAsync("/employee/updateEmployee", requestUpdate);

            Assert.StrictEqual(HttpStatusCode.NotFound, resultUpdate.StatusCode);
        }

        [Fact]
        public async Task UpdateEmployeeInvalidName()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 85,
                EndDate = DateTime.Now.AddMonths(10)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeResponse);

            var requestUpdate = new UpdateEmployeeCommand
            {
                Id = employeeResponse.Id,
                Name = "Jane Do3"
            };

            var resultUpdate = await _httpClient.PutAsJsonAsync("/employee/updateEmployee", requestUpdate);
            Assert.StrictEqual(HttpStatusCode.BadRequest, resultUpdate.StatusCode);

            _ = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{employeeResponse.Id}");
        }

        [Fact]
        public async Task UpdateEmployeeInvalidBaseSalary()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 85,
                EndDate = DateTime.Now.AddMonths(10)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeResponse);

            var requestUpdate = new UpdateEmployeeCommand
            {
                Id = employeeResponse.Id,
                BaseSalary = -100
            };

            var resultUpdate = await _httpClient.PutAsJsonAsync("/employee/updateEmployee", requestUpdate);
            Assert.StrictEqual(HttpStatusCode.BadRequest, resultUpdate.StatusCode);

            _ = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{employeeResponse.Id}");
        }

        [Fact]
        public async Task DeleteEmployeeValid()
        {
            var request = new CreateEmployeeCommand
            {
                Name = "John Doe",
                BaseSalary = 85,
                EndDate = DateTime.Now.AddMonths(10)
            };

            var result = await _httpClient.PostAsJsonAsync("/employee/createEmployee", request);

            result.EnsureSuccessStatusCode();

            var employeeResponse = await result.Content.ReadFromJsonAsync<EmployeeDto>();
            Assert.NotNull(employeeResponse);

            var resultDelete = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{employeeResponse.Id}");
            Assert.StrictEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task DeleteEmployeeNotFound()
        {
            var resultDelete = await _httpClient.DeleteAsync($"/employee/deleteEmployee/{new Guid()}");
            Assert.StrictEqual(HttpStatusCode.NotFound, resultDelete.StatusCode);
        }
    }

    public record Login
    {
        public string token { get; set; }
    }
}