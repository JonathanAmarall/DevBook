using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using SilkierQuartz;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSwaggerGenWithAuth();

        services.AddSilkierQuartz(options =>
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

        // REMARK: If you want to use Controllers, you'll need this.
        services.AddControllers();

        services.Configure<JsonOptions>(options =>
          options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        return services;
    }
}
