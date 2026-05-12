using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using APP.Application.Identity;
using APP.Infrastructure.Identity;

namespace APP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IIdentityStore, SqlIdentityStore>();

        return services;
    }
}
