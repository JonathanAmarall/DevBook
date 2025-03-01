using System.Reflection;
using System.Text.Json;
using Application.Abstractions.Behaviors;
using Application.ExternalServices.Github;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddRefitClient<IGithubAuthApi>(new RefitSettings
        {
            ContentSerializer = new FormUrlEncodedDeserializer()
        }).ConfigureHttpClient(c =>
        {
#pragma warning disable S1075 // URIs should not be hardcoded
            c.BaseAddress = new Uri("https://github.com");
#pragma warning restore S1075 // URIs should not be hardcoded
            c.DefaultRequestHeaders.Add("User-Agent", "LogDevApi"); // GitHub exige um User-Agent            
        });

        services.AddRefitClient<IGithubApi>(new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer()
        }).ConfigureHttpClient(c =>
        {
#pragma warning disable S1075 // URIs should not be hardcoded
            c.BaseAddress = new Uri("https://api.github.com/");
#pragma warning restore S1075 // URIs should not be hardcoded
            c.DefaultRequestHeaders.Add("User-Agent", "LogDevApi"); // GitHub exige um User-Agent
            //c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}

public class FormUrlEncodedDeserializer : IHttpContentSerializer
{
    public async Task<T?> DeserializeAsync<T>(HttpContent content)
    {
        string data = await content.ReadAsStringAsync();

        var dict = data.Split('&')
            .Select(p => p.Split('='))
            .ToDictionary(k => k[0], v => Uri.UnescapeDataString(v[1]));

        string json = JsonSerializer.Serialize(dict);

        return JsonSerializer.Deserialize<T>(json);
    }

    public Task<HttpContent> SerializeAsync<T>(T item)
    {
        throw new NotImplementedException("Este serializer só é usado para desserialização.");
    }

    public HttpContent ToHttpContent<T>(T item)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
    {
        string data = await content.ReadAsStringAsync(cancellationToken);

        var dict = data.Split('&')
            .Select(p => p.Split('='))
            .ToDictionary(k => k[0], v => Uri.UnescapeDataString(v[1]));

        string json = JsonSerializer.Serialize(dict);

        return JsonSerializer.Deserialize<T>(json);
    }

    public string? GetFieldNameForProperty(PropertyInfo propertyInfo)
    {
        throw new NotImplementedException();
    }

    public string? ContentType => "application/x-www-form-urlencoded";
}
