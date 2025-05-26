using Infrastructure.ExternalServices.Gemini;
using Refit;

namespace Web.Api.Endpoints.Summary;

public class DailySummary : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/daily-summary",
            async (ContentsRequest request, IGeminiApi api, CancellationToken cancellationToken) =>
            {
                ApiResponse<ContentsResponse> response = await api.GenerateContentAsync(
                    request: request,
                    cancellation: cancellationToken);

                return response.IsSuccessStatusCode
                    ? Results.Ok(response.Content)
                    : Results.Problem(
                        response.Error?.Content,
                        statusCode: (int)response.StatusCode,
                        title: response.Error?.Message);
            })
        .WithTags(Tags.Summary);
    }
}
