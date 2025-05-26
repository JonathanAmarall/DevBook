using Domain.Services;

namespace Infrastructure.Services.TextGeneration;

public class OpenAiTextGenerationService : ITextSummaryGeneratorService
{
    private const string Sample = "Implement OpenAI text generation logic here";

    public Task<string> GenerateTextSummaryAsync(string text, CancellationToken cancellationToken)
    {
        return Task.FromResult(Sample);
    }
}
