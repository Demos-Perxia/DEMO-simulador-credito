using CuotaClara.Application.Abstractions;
using CuotaClara.Infrastructure.Catalogs;
using CuotaClara.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CuotaClara.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CreditPolicyOptions>(configuration.GetSection(CreditPolicyOptions.SectionName));
        services.AddSingleton<ICatalogRepository, InMemoryCatalogRepository>();
        services.AddSingleton<ICreditPolicyProvider, ConfiguredCreditPolicyProvider>();
        return services;
    }
}
