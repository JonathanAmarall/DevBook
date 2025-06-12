using Application.Entries.Complete;
using Application.Entries.GetById;
using MediatR;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Entries;

public class Resolved : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/entries/{id}/resolved",
            async (string id, ISender sender, CancellationToken cancellationToken) =>
        {
            var request = new MarkEntryAsResolvedCommand(id);
            Result<EntryResponse> result = await sender.Send(request, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Entries);
    }
}
