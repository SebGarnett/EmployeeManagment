using System.Text;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(setup =>
{
   setup.EnableAnnotations();
   setup.SwaggerDoc("v1", new OpenApiInfo
   {
       Version = "v1",
       Title = "Employee Management API",
       Description = "This is the employee management API"
   });
   setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
       Type = SecuritySchemeType.ApiKey,
       In = ParameterLocation.Header,
       Name = HeaderNames.Authorization,
       Scheme = "Bearer",
       Description = "Enter your JWT token in this format : {token}"
   });
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
       {
           {
               new OpenApiSecurityScheme
               {
                   Reference = new OpenApiReference
                   {
                       Type= ReferenceType.SecurityScheme,
                       Id = "Bearer"
                   },
                   Scheme = "oauth2",
                   Name = "Bearer",
                   In = ParameterLocation.Header
               },
               new string[]{}
           }
       }
    );
});

var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var apiKey = builder.Configuration["Jwt:ApiKey"];

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

if (issuer is not null && audience is not null && apiKey is not null)
{
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = "employeeManagement",
                ValidAudience = "myClient",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("q8X5ZAMFk1C/GUf7OtuwERy6bZFY4OtvNl8rkpp+vKdx9wkO1lxSzdZXoRAPDKNk"))
            };
        });
}
else
    throw new Exception("Jwt not configured !");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

public partial class Program{}