using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.API
{
    public static class JwtTokenGenerator
    {
        
        public static string GenerateJwtToken(string issuer, string audience, string apiKey)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(apiKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                signingCredentials: cred,
                expires: DateTime.Now.AddMinutes(10)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
