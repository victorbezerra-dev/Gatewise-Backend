using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class GatewiseClientHandler : AuthorizationHandler<GatewiseClientRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GatewiseClientRequirement requirement)
    {
        var clientId = context.User.FindFirst("azp")?.Value ?? context.User.FindFirst("client_id")?.Value;

        if (clientId == requirement.RequiredClientId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
