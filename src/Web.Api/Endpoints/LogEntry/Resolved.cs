using Application.Entries.GetById;
using Application.LogEntry.Complete;
using MediatR;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.LogEntry;

public class Resolved : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/log-entry/{logId}/resolved", async (string logId, ISender sender, CancellationToken cancellationToken) =>
        {
            var request = new MarkEntryAsResolvedCommand(logId);
            Result<EntryResponse> result = await sender.Send(request, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.LogEntry);
    }
}
