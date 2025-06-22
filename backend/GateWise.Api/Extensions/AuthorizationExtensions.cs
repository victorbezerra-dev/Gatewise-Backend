using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace GateWise.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireClientIdGatewiseSync", policy =>
                policy.RequireClaim("azp", "gatewise-user-sync"));
        });

        return services;
    }
}
