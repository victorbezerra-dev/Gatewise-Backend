using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using System.Threading;
using System.Threading.Tasks;

namespace GateWise.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
            options.AddDocumentTransformer<BearerSecurityTransformer>();
        });

        return services;
    }
}

internal sealed class BearerSecurityTransformer(IAuthenticationSchemeProvider schemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Servers = new List<OpenApiServer>
        {
            new() { Url = "http://localhost:8081" }
        };

        var schemes = await schemeProvider.GetAllSchemesAsync();
        if (schemes.Any(s => s.Name == "Bearer"))
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JWT"
            };

            if (document.Paths is not null)
            {
                foreach (var operation in document.Paths.SelectMany(p => p.Value.Operations))
                {
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }] = Array.Empty<string>()
                    });
                }
            }
        }
    }
}
