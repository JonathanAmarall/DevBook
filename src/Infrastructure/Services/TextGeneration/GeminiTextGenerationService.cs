using Domain.Services;
using Infrastructure.ExternalServices.Gemini;
using Refit;

namespace Infrastructure.Services.TextGeneration;
public class GeminiTextGenerationService : ITextSummaryGeneratorService
{
    private readonly IGeminiApi _geminiApi;

    public GeminiTextGenerationService(IGeminiApi geminiApi)
    {
        _geminiApi = geminiApi;
    }

    public async Task<string> GenerateTextSummaryAsync(string text, CancellationToken cancellationToken)
    {
        var request = new ContentsRequest
        {
            Contents =
            [
                new ContentRequest
                {
                    Parts = [new PartRequest { Text = text }]
                }
            ]
        };

        ApiResponse<ContentsResponse> response = await _geminiApi
            .GenerateContentAsync(request, cancellationToken);

        return response?
                .Content?
                .Candidates
                .FirstOrDefault()?
                .Content
                .Parts
                .FirstOrDefault()?
                .Text ?? "";
    }
}
