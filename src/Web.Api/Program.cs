using System.Reflection;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Web.Api;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options
    => options.AddPolicy("Cors",
        builder => builder
                .WithOrigins("http://localhost:4200", "https://jonathanamarall.github.io")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services
    .AddApplication(builder.Configuration)
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

app.UseCors("Cors");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseSilkierQuartz();

app.MapEndpoints();

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Web.Api
{
    public partial class Program;
}
