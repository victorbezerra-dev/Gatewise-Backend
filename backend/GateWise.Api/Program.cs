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

app.Use(async (context, next) =>
{
    await next();

    context.Response.ContentType = "application/json";

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        await context.Response.WriteAsync("""
        { "error": "forbidden", "message": "You are not authorized to access this resource." }
        """);
    }
    else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
    {
        await context.Response.WriteAsync("""
        { "error": "unauthorized", "message": "Authentication token is missing or invalid." }
        """);
    }
});

app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();