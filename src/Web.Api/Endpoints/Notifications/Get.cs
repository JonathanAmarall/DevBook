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
            [FromQuery] short pageSize,
            [FromQuery] short pageNumber,
            ISender sender, CancellationToken cancellationToken) =>
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
        .WithTags(Tags.Notifications);
    }
}
