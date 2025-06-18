using Domain.Notifications;
using Domain.Repositories;
using Infrastructure.Quartz.Scheduling;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Quartz.Jobs;
public class SendNotificationJob : IJob
{
    private readonly ILogger<SendNotificationJob> _logger;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationDeliveryRepository _notificationDeliveryRepository;

    public SendNotificationJob(ILogger<SendNotificationJob> logger, INotificationRepository notificationRepository, INotificationDeliveryRepository notificationDeliveryRepository)
    {
        _logger = logger;
        _notificationRepository = notificationRepository;
        _notificationDeliveryRepository = notificationDeliveryRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Executing SendNotificationJob at {Time}", DateTimeOffset.Now);

            if (!context.MergedJobDataMap.TryGetString(QuartzScheduleFactory.NotificationKey, out string notificationId) || string.IsNullOrEmpty(notificationId))
            {
                _logger.LogError("Notification ID is missing or invalid.");
                return;
            }

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

            var delivery = new NotificationDelivery(
                notification.Id,
                notification.UserId,
                NotificationChannel.InApp,
                notification.Title);

            delivery.MarkAsSent();

            await _notificationDeliveryRepository.UnitOfWork.StartTransactionAsync(context.CancellationToken);
            await _notificationDeliveryRepository.AddAsync(delivery, context.CancellationToken);
            await _notificationDeliveryRepository.UnitOfWork.CommitChangesAsync(context.CancellationToken);

            _logger.LogInformation("Notification with ID {NotificationId} has been sent and delivery registered.", notificationId);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
