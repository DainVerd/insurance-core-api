using Application.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddSingleton<IClaimRepository, ClaimRepository>();
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IPolicyRepository, PolicyRepository>();


        return services;
    }
}
