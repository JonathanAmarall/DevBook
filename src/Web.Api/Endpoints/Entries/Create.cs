using Application.Entries.Create;
using Application.Entries.GetById;
using MediatR;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Entries;

public class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/entries",
            async (CreateEntryCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<EntryResponse> response = await sender.Send(request, cancellationToken);

                return response.Match(Results.Ok, CustomResults.Problem);
            })
        .WithTags(Tags.Entries);
    }
}
