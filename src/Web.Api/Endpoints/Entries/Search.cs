using Application.Entries.Search;
using Domain.Entries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Entries;

public class Search : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/entries",
            async (
                ISender sender,
                [FromQuery] string? title,
                [FromQuery] string[]? tags,
                [FromQuery] EntryCategory? category,
                [FromQuery] EntryStatus? status,
                [FromQuery] short? pageNumber = 1,
                [FromQuery] short? pageSize = 10,
                CancellationToken cancellationToken = default) =>
            {
                Result<PagedList<SearchEntryQueryResponse>> response =
                    await sender.Send(new SearchEntryQuery(title, category, [.. tags], status, default) { PageNumber = pageNumber, PageSize = pageSize }, cancellationToken);

                return response.Match(Results.Ok, Results.NotFound);
            })
            //.HasPermission(Permissions.UsersAccess)
            .WithTags(Tags.Entries);
    }
}
