using System.Net;
using System.Net.Http.Json;
using EmployeeManagement.Application.EmployeeItems.Command.Create;
using EmployeeManagement.Application.EmployeeItems.Command.Update;
using EmployeeManagement.Application.EmployeeItems.Dtos;
using EmployeeManagement.Application.EmployeeItems.Query;
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EmployeeManagement.IntegrationTests
{
    public class EmployeeManagementControllerTests
    {
        private readonly HttpClient _httpClient;

        public EmployeeManagementControllerTests()
        {
            var webAppClientFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppClientFactory.CreateClient();
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
                Id = employeeResponse.Id,
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
    }
}