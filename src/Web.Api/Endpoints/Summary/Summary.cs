using Domain.Users;
using Infrastructure.DomainEvents;
using Infrastructure.ExternalServices.Gemini;

namespace Web.Api.Endpoints.Summary;

public class Summary : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/summary",
            async (ContentsRequest request, IGeminiApi api, IDomainEventsDispatcher domainEventsDispatcher, CancellationToken cancellationToken) =>
            {
                await domainEventsDispatcher.DispatchAsync([new UserRegisteredDomainEvent("xpto")], cancellationToken);
                return Results.Ok();
                //ApiResponse<ContentsResponse> response = await api.GenerateContentAsync(
                //    request: request,
                //    cancellation: cancellationToken);

                //return response.IsSuccessStatusCode
                //    ? Results.Ok(response.Content)
                //    : Results.Problem(
                //        response.Error?.Content,
                //        statusCode: (int)response.StatusCode,
                //        title: response.Error?.Message);
            })
        .WithTags(Tags.Summary);
    }
}
