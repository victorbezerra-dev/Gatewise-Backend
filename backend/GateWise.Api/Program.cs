using GateWise.Api.Extensions;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using GateWise.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});
builder.Services.AddSingleton<IAuthorizationHandler, GatewiseClientHandler>();
builder.Services.AddScoped<IClaimsTransformation, KeycloakClaimsTransformer>();
builder.Services.AddControllers();

builder.Services
    .AddJwtAuthentication()
    .AddCustomAuthorization()
    .AddCustomOpenApi();


var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi/v1.json", "GateWise API v1");
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});


app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    response.ContentType = "application/json";

    if (response.StatusCode == StatusCodes.Status401Unauthorized)
    {
        await response.WriteAsync("""{ "error": "unauthorized", "message": "Authentication token is missing or invalid." }""");
    }
    else if (response.StatusCode == StatusCodes.Status403Forbidden)
    {
        await response.WriteAsync("""{ "error": "forbidden", "message": "You are not authorized to access this resource." }""");
    }
});  

app.UseCustomExceptionHandler();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();