using Application.LogBook.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.LogEntry;

public class Search : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/log-entry",
            async ([FromQuery] string? title, [FromQuery] string[]? tags, [FromQuery] short? pageNumber, [FromQuery] short? pageSize, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<PagedList<SearchLogEntryQueryResponse>> response =
                    await sender.Send(new SearchLogEntryQuery(title, default, [.. tags], default, default) { PageNumber = pageNumber, PageSize = pageSize }, cancellationToken);

                return response.Match(Results.Ok, Results.NotFound);
            })
            //.HasPermission(Permissions.UsersAccess)
            .WithTags(Tags.LogEntry);
    }
}
