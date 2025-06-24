using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

public class KeycloakClaimsTransformer : IClaimsTransformation
{
    private readonly ILogger<KeycloakClaimsTransformer> _logger;

    public KeycloakClaimsTransformer(ILogger<KeycloakClaimsTransformer> logger)
    {
        _logger = logger;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(c => c.Type == ClaimTypes.Role))
        {
            _logger.LogDebug("Role claims already present. Skipping transformation.");
            return Task.FromResult(principal);
        }

        if (principal.Identity is not ClaimsIdentity identity)
        {
            _logger.LogWarning("Principal identity is not a ClaimsIdentity. Skipping transformation.");
            return Task.FromResult(principal);
        }

        AddRealmRoles(identity);

        return Task.FromResult(principal);
    }

    private void AddRealmRoles(ClaimsIdentity identity)
    {
        var realmAccessClaim = identity.FindFirst("realm_access");
        if (realmAccessClaim is null || string.IsNullOrWhiteSpace(realmAccessClaim.Value))
        {
            _logger.LogWarning("No 'realm_access' claim found or it's empty.");
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(realmAccessClaim.Value);
            if (document.RootElement.TryGetProperty("roles", out var roles))
            {
                foreach (var role in roles.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                        _logger.LogInformation("Added role claim: {Role}", roleName);
                    }
                    else
                    {
                        _logger.LogWarning("Found empty or null role name in 'realm_access'.");
                    }
                }
            }
            else
            {
                _logger.LogWarning("'realm_access' claim does not contain a 'roles' array.");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse 'realm_access' claim JSON.");
        }
    }
}
