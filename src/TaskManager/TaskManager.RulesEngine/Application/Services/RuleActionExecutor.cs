using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Reflection;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleActionExecutor(IMediator mediator, ILogger<RuleActionExecutor> logger) : IRuleActionExecutor
{
    public async Task ExecuteAsync(TaskItem task, RuleAction action)
    {
        switch (action)
        {
            case SetFieldAction set:
                ApplySetField(task, set);
                break;

            case CreateNotificationAction notif:
                await CreateNotification(task, notif);
                break;

            case CreateCalendarEventAction calendar:
                await CreateCalendar(task, calendar);
                break;
        }
    }

    private void ApplySetField(TaskItem task, SetFieldAction set)
    {
        var prop = task.GetType().GetProperty(set.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop == null || !prop.CanWrite) return;
        logger.LogDebug("Setting field {Field} to value {Value} on TaskId: {TaskId}", set.Field, set.Value, task.TaskId);

        Type targetType = prop.PropertyType;

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        object? value;

        if (underlyingType == typeof(Guid))
        {
            var setValue = set.Value;
            value = string.IsNullOrEmpty(setValue) ? null : Guid.Parse(setValue);
        }
        else if (underlyingType == typeof(DateTimeOffset))
        {
            var setValue = set.Value;
            value = string.IsNullOrEmpty(setValue) ? null : DateTimeOffset.Parse(setValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
        else if (underlyingType.IsEnum)
        {
            value = Enum.Parse(underlyingType, set.Value, true);
        }
        else
        {
            value = Convert.ChangeType(set.Value, underlyingType);
        }

        prop.SetValue(task, value);
    }

    private async Task CreateNotification(TaskItem task, CreateNotificationAction notif)
    {
        var baseDate = task.DueDate;

        if (baseDate == null)
        {
            logger.LogWarning("Task {TaskId} has no DueDate for notification", task.TaskId);
            return;
        }

        var dueDate = (DateTimeOffset)baseDate;

        var scheduledAt = dueDate.AddMinutes(-notif.OffsetMinutes);

        await mediator.Send(new CreateNotificationCommand(
            task.OwnerId,
            notif.ServiceId,
            notif.Description,
            scheduledAt,
            task.TaskId
        ));

        logger.LogDebug(
            "Created notification for TaskId {TaskId} at {ScheduledAt}",
            task.TaskId,
            scheduledAt
        );
    }

    private async Task CreateCalendar(TaskItem task, CreateCalendarEventAction calendar)
    {
        var baseDate = task.DueDate;

        if (baseDate == null)
        {
            logger.LogWarning("Task {TaskId} has no DueDate for calendar event", task.TaskId);
            return;
        }

        var startTime = (DateTimeOffset)baseDate;

        var endTime = startTime.AddMinutes(calendar.DurationMinutes);

        await mediator.Send(new CreateCalendarEventCommand(
            task.OwnerId,
            calendar.Title ?? "Новое событие",
            startTime,
            endTime,
            calendar.Location,
            task.TaskId,
            string.IsNullOrWhiteSpace(calendar.ExternalAccountId) 
                ? null
                : Guid.Parse(calendar.ExternalAccountId)
        ));

        logger.LogDebug(
            "Created calendar event for TaskId {TaskId} from {Start} to {End}",
            task.TaskId,
            startTime,
            endTime
        );
    }
}
