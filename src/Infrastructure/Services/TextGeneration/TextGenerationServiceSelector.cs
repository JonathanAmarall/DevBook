using Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.TextGeneration;
public sealed class TextGenerationServiceSelector : ITextSummaryGeneratorService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public TextGenerationServiceSelector(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public Task<string> GenerateTextSummaryAsync(string text, CancellationToken cancellationToken)
    {
        string? provider = _configuration["AiProvider"]; // "Gemini" ou "OpenAI"

        GeminiTextGenerationService service = provider switch
        {
            "Gemini" => _serviceProvider.GetRequiredService<GeminiTextGenerationService>(),
            _ => throw new InvalidOperationException("Invalid AI provider")
        };

        return service.GenerateTextSummaryAsync(text, cancellationToken);
    }
}
