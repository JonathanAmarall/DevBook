using Application.Summaries.Daily;
using MediatR;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Summaries;

public class Daily : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/summaries/daily",
            async (ISender sender, CancellationToken cancellationToken) =>
        {
            Result<DailySummaryResponse> result = await sender.Send(new DailySummaryQuery(), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .Produces<DailySummaryResponse>()
        .WithTags(Tags.Summary);
    }
}
