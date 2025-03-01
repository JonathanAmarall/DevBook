using Application.LogBook.GetById;
using MediatR;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.LogEntry;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/log-entry/{id}",
            async (string id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<LogEntryResponse> response =
                    await sender.Send(new GetLogEntryByIdQuery(id), cancellationToken);

                return response.Match(Results.Ok, Results.NotFound);
            })
            .HasPermission(Permissions.UsersAccess)
            .WithTags(Tags.LogEntry);
    }
}
