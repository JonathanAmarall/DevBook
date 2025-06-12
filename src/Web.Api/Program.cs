using System.Reflection;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SilkierQuartz;
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

builder.Services.AddSwaggerGenWithAuth();

builder.Services.AddSilkierQuartz(options =>
{
    options.VirtualPathRoot = "/quartz";
    options.UseLocalTime = true;
    options.CronExpressionOptions = new CronExpressionDescriptor.Options()
    {
        DayOfWeekStartIndexZero = false //Quartz uses 1-7 as the range
    };
    options.EnableEdit = false;
}, configureAuthenticationOptions: authenticationOptions
    => authenticationOptions.AccessRequirement = SilkierQuartzAuthenticationOptions.SimpleAccessRequirement.AllowAnonymous);


builder.Services
    .AddApplication(builder.Configuration)
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

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
