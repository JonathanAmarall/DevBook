using Application.Entries.GetById;
using MediatR;
using SharedKernel;
using Web.Api.Endpoints.Users;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Entries;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/entries/{id}",
            async (string id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<EntryResponse> response =
                    await sender.Send(new GetEntryByIdQuery(id), cancellationToken);

                return response.Match(Results.Ok, Results.NotFound);
            })
            .HasPermission(Permissions.UsersAccess)
            .WithTags(Tags.Entries);
    }
}
