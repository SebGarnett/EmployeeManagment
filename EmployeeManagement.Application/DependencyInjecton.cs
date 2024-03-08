using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace EmployeeManagement.Application
{
    public static class DependencyInjecton
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(option => 
                option.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<IMediator,Mediator>();
        }
    }
}
