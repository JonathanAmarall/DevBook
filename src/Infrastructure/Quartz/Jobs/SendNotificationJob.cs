using Domain.Notifications;
using Domain.Repositories;
using Infrastructure.Quartz.Scheduling;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Quartz.Jobs;
public class SendNotificationJob : IJob
{
    private readonly ILogger<SendNotificationJob> _logger;
    private readonly INotificationRespository _notificationRepository;
    private readonly INotificationDeliveryRepository _deliveryRepository;
    public SendNotificationJob(ILogger<SendNotificationJob> logger, INotificationRespository notificationRepository, INotificationDeliveryRepository deliveryRepository)
    {
        _logger = logger;
        _notificationRepository = notificationRepository;
        _deliveryRepository = deliveryRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing SendNotificationJob at {Time}", DateTimeOffset.Now);

        context.MergedJobDataMap.TryGetString(QuartzScheduleFactory.NotificationKey, out string notificationId);

        _logger.LogInformation("Executing notification job for ID: {NotificationId}", notificationId);

        Notification? notification = await _notificationRepository.FirstOrDefaultAsync(
            n => n.Id.ToString() == notificationId,
            cancellationToken: context.CancellationToken
        );

        if (notification is null)
        {
            _logger.LogWarning("Notification with ID {NotificationId} not found.", notificationId);
            return;
        }

        // Aqui você pode enviar a notificação (e-mail, push, etc.)

        // Cria o registro de histórico
        var delivery = new NotificationDelivery
        {
            NotificationId = notification.Id,
            RecipientId = notification.UserId,
            Channel = NotificationChannel.InApp,
            SentOnUtc = DateTime.UtcNow,
            Status = NotificationStatus.Sent,
            IsRead = false
        };

        await _deliveryRepository.AddAsync(delivery, context.CancellationToken);

        _logger.LogInformation("Notification with ID {NotificationId} has been sent and delivery registered.", notificationId);
    }
}
