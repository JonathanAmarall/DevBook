using Application.Abstractions.Messaging;
using Application.Notifications.Get;

namespace Application.Notifications.MarkAsRead;
public sealed record MarkAsReadNotificationCommand(string Id) : ICommand<NotificationResponse>;
