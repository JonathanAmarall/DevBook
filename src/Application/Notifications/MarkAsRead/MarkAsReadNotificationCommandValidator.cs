using FluentValidation;

namespace Application.Notifications.MarkAsRead;

public class MarkAllAsReadNotificationCommandValidator : AbstractValidator<MarkAsReadNotificationCommand>
{
    public MarkAllAsReadNotificationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Notification ID cannot be empty.");
    }

}
