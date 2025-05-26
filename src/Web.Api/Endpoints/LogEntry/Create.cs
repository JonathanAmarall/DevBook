using Application.LogEntry.Create;
using Application.LogEntry.GetById;
using MediatR;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.LogEntry;

public class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/log-entry",
            async (CreateLogEntryCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<LogEntryResponse> response = await sender.Send(request, cancellationToken);

                return response.Match(Results.Ok, CustomResults.Problem);
            })
        .WithTags(Tags.LogEntry);
    }
}
