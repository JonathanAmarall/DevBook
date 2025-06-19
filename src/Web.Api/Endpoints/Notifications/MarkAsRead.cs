using Application.Notifications.Get;
using Application.Notifications.MarkAsRead;
using MediatR;
using SharedKernel;
using Web.Api.Endpoints.Users;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Notifications;

public class MarkAsRead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/v1/notifications/{id}/read", async (
            string id,
            ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new MarkAsReadNotificationCommand(id);
            Result<NotificationResponse> result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.UsersAccess)
        .Produces<PagedList<NotificationResponse>>()
        .WithTags(Tags.Notifications);
    }
}
