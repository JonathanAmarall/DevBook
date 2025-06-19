using Application.Notifications.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Endpoints.Users;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Notifications;

public class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v1/notifications", async (
            ISender sender, CancellationToken cancellationToken,
            [FromQuery] short? pageSize = 10,
            [FromQuery] short? pageNumber = 1) =>
        {
            var query = new GetNotificationsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Result<PagedList<NotificationResponse>> result = await sender.Send(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersAccess)
        .Produces<PagedList<NotificationResponse>>()
        .WithTags(Tags.Notifications);
    }
}
