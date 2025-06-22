using Microsoft.AspNetCore.Authorization;

public class GatewiseClientRequirement : IAuthorizationRequirement
{
    public string RequiredClientId { get; }

    public GatewiseClientRequirement(string requiredClientId)
    {
        RequiredClientId = requiredClientId;
    }
}
