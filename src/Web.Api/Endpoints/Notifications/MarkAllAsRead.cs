using Application.Notifications.MarkAsRead;
using MediatR;
using SharedKernel;
using Web.Api.Endpoints.Users;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Notifications;

public class MarkAllAsRead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/notifications/read-all", async (
            ISender sender, CancellationToken cancellationToken) =>
        {
            Result result = await sender.Send(
                new MarkAllAsReadNotificationCommand(), cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersAccess)
        .Produces(StatusCodes.Status204NoContent)
        .WithTags(Tags.Notifications);
    }
}
