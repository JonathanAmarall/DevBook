using Refit;

namespace Infrastructure.ExternalServices.Gemini;
public interface IGeminiApi
{
    [Post("/v1beta/models/gemini-2.0-flash:generateContent")]
    Task<ApiResponse<ContentsResponse>> GenerateContentAsync(
        [Body] ContentsRequest request, CancellationToken cancellation);
}

