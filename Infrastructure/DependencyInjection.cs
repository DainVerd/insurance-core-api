using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<IClaimRepository, ClaimRepository>();
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IPolicyRepository, PolicyRepository>();

        services.AddTransient<ICustomerService, CustomerService>();
        services.AddTransient<IPolicyService, PolicyService>();
        services.AddTransient<IClaimService, ClaimService>();

        return services;
    }
}
