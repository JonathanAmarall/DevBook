using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Quartz.Jobs;
public class SendNotificationJob : IJob
{
    private readonly ILogger<SendNotificationJob> _logger;

    public SendNotificationJob(ILogger<SendNotificationJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        context.MergedJobDataMap.TryGetString("NotificationId", out string notificationId);
        _logger.LogInformation("Executing notification job for ID: {NotificationId}", notificationId);
        await Task.CompletedTask; // Simulate sending notification logic here
    }
}
