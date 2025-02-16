using Application.LogBook.Create;
using MediatR;

namespace Web.Api.Endpoints.LogEntry;

public class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/log-entry",
            async (CreateLogEntryCommand request, ISender sender, CancellationToken cancellationToken) =>
                Results.Ok(await sender.Send(request, cancellationToken)))
        .WithTags(Tags.LogEntry);
    }
}
