using EmployeeManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("auth/login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (IsCredentialsValid(model))
            {
                var token = JwtTokenGenerator.GenerateJwtToken(_configuration["Jwt:Issuer"]!, _configuration["Jwt:Audience"]!, _configuration["Jwt:ApiKey"]!);

                return Ok(new { token });
            }

            return Unauthorized();
        }

        private bool IsCredentialsValid(LoginModel model)
        {
            return (model.Username == "Test" && model.Password == "TestPassword");
        }
    }
}
