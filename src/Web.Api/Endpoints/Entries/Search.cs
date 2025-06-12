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
                [FromQuery] string? title,
                [FromQuery] string[]? tags,
                [FromQuery] short? pageNumber,
                [FromQuery] short? pageSize,
                [FromQuery] EntryCategory? category,
                [FromQuery] EntryStatus? status,
                ISender sender, CancellationToken cancellationToken) =>
            {
                Result<PagedList<SearchEntryQueryResponse>> response =
                    await sender.Send(new SearchEntryQuery(title, category, [.. tags], status, default) { PageNumber = pageNumber, PageSize = pageSize }, cancellationToken);

                return response.Match(Results.Ok, Results.NotFound);
            })
            //.HasPermission(Permissions.UsersAccess)
            .WithTags(Tags.Entries);
    }
}
