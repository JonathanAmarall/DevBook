using System.Reflection;
using System.Text;
using System.Text.Json;
using Application;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Schedulers;
using Application.Summaries.Services;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.Database.Context;
using Infrastructure.Database.Repositories;
using Infrastructure.Database.UnitOfWork;
using Infrastructure.DomainEvents;
using Infrastructure.ExternalServices.Gemini;
using Infrastructure.ExternalServices.Github;
using Infrastructure.Quartz.Scheduling;
using Infrastructure.Services.TextGeneration;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Quartz;
using Refit;
using SharedKernel;

namespace Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices(configuration)
            .AddDatabase(configuration)
            .InsertHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddGithubOAuthProvider()
            .AddAuthorizationInternal()
            .AddTextSummaryGenerator()
            .AddBackgroundJobs()
            .AddRedisCache(configuration);

    public static void UseRepositoryInitializers(this IApplicationBuilder app)
    {
        foreach (IMappingConfigurationEntity initializer in app.ApplicationServices.GetServices<IMappingConfigurationEntity>())
        {
            initializer.InitializeAsync().Wait();
        }
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped<ISummaryEntryService, SummaryEntryService>();

        services.AddRefitClients(configuration);

        return services;
    }

    private static void AddRefitClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRefitClient<IGithubAuthApi>(new RefitSettings
        {
            ContentSerializer = new FormUrlEncodedDeserializer()
        }).ConfigureHttpClient(c =>
        {
            c.BaseAddress = new Uri(configuration["GithubAuthApiSettings:BaseAddress"]!);
            c.DefaultRequestHeaders.Add("User-Agent", "LogDevApi");
        });

        services.AddRefitClient<IGithubApi>(new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            })
        }).ConfigureHttpClient(c =>
        {
            c.BaseAddress = new Uri(configuration["GithubApiSettings:BaseAddress"]!);
            c.DefaultRequestHeaders.Add("User-Agent", "LogDevApi");
        });

        services.AddTransient<GeminiApiKeyHandler>();

        services.AddRefitClient<IGeminiApi>()
            .ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration["GeminiApiSettings:BaseAddress"]!))
            .AddHttpMessageHandler<GeminiApiKeyHandler>();
    }

    private static IServiceCollection AddGithubOAuthProvider(this IServiceCollection services)
    {
        services.AddTransient<IOAuthProvider, GithubOAuthProvider>();
        return services;
    }

    private static IServiceCollection AddTextSummaryGenerator(this IServiceCollection services)
    {
        services.AddTransient<GeminiTextGenerationService>();
        services.AddTransient<OpenAiTextGenerationService>();
        services.AddTransient<ITextSummaryGeneratorService, TextGenerationServiceSelector>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<IDatabaseContext, MongoDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddRepositoryMappings(Assembly.GetCallingAssembly());
        services.AddRepositoryInitializers(Assembly.GetCallingAssembly());

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationScheduleRepository, NotificationScheduleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEntryRepository, EntryRepository>();
        services.AddScoped<INotificationDeliveryRepository, NotificationDeliveryRepository>();

        return services;
    }

    private static IServiceCollection InsertHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton(sp => new MongoClient(configuration["MongoDbSettings:ConnectionString"]))
            .AddHealthChecks()
            .AddMongoDb();

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz();

        services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

        services.AddScoped<INotificationScheduler, NotificationScheduler>();

        return services;
    }

    private static void AddRepositoryMappings(this IServiceCollection services, Assembly assemblyToScan)
          => MongoDbMappings.RegisterMappings(assemblyToScan);

    private static void AddRepositoryInitializers(this IServiceCollection services, Assembly assemblyToScan)
    {
        Type initializerType = typeof(IMappingConfigurationEntity);
        IEnumerable<Type> types = assemblyToScan.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && initializerType.IsAssignableFrom(t));

        foreach (Type type in types)
        {
            services.AddSingleton(initializerType, type);
        }
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        return services;
    }
}
