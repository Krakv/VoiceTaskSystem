using MediatR;
using NSubstitute;
using Microsoft.Extensions.Logging;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.RulesEngine.Application.Services;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using FluentAssertions;

namespace TaskManager.UnitTests.RuleEngine;

public class RuleActionExecutorTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ILogger<RuleActionExecutor> _logger = Substitute.For<ILogger<RuleActionExecutor>>();
    private readonly RuleActionExecutor _executor;

    public RuleActionExecutorTests()
    {
        _executor = new RuleActionExecutor(_mediator, _logger);
    }

    #region SetField

    [Fact]
    public async Task Should_set_string_field()
    {
        var task = CreateTask(title: "Old");

        var action = new SetFieldAction
        {
            Field = "Title",
            Value = "New"
        };

        await _executor.ExecuteAsync(task, action);

        task.Title.Should().Be("New");
    }

    [Fact]
    public async Task Should_set_enum_field()
    {
        var task = CreateTask(status: TaskItemStatus.New);

        var action = new SetFieldAction
        {
            Field = "Status",
            Value = "Done"
        };

        await _executor.ExecuteAsync(task, action);

        task.Status.Should().Be(TaskItemStatus.Done);
    }

    [Fact]
    public async Task Should_set_guid_field()
    {
        var id = Guid.NewGuid();
        var task = CreateTask(parentId: null);

        var action = new SetFieldAction
        {
            Field = "ParentTaskId",
            Value = id.ToString()
        };

        await _executor.ExecuteAsync(task, action);

        task.ParentTaskId.Should().Be(id);
    }

    [Fact]
    public async Task Should_set_datetime_field()
    {
        var date = DateTimeOffset.UtcNow;
        var task = CreateTask(dueDate: null);

        var action = new SetFieldAction
        {
            Field = "DueDate",
            Value = date.ToString("O")
        };

        await _executor.ExecuteAsync(task, action);

        task.DueDate.Should().Be(date);
    }

    [Fact]
    public async Task Should_ignore_when_field_not_exists()
    {
        var task = CreateTask(title: "Test");

        var action = new SetFieldAction
        {
            Field = "Unknown",
            Value = "Value"
        };

        await _executor.ExecuteAsync(task, action);

        task.Title.Should().Be("Test");
    }

    #endregion

    #region Notification

    [Fact]
    public async Task Should_send_notification_command()
    {
        var dueDate = DateTimeOffset.UtcNow;

        var task = CreateTask(dueDate: dueDate);

        var action = new CreateNotificationAction
        {
            Description = "Test notification",
            OffsetMinutes = 10
        };

        await _executor.ExecuteAsync(task, action);

        await _mediator.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(cmd =>
                cmd.Description == "Test notification" &&
                cmd.TaskId == task.TaskId
            ),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_calculate_notification_time_correctly()
    {
        var dueDate = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero);

        var task = CreateTask(dueDate: dueDate);

        var action = new CreateNotificationAction
        {
            Description = "Test",
            OffsetMinutes = 60
        };

        await _executor.ExecuteAsync(task, action);

        await _mediator.Received().Send(
            Arg.Is<CreateNotificationCommand>(cmd =>
                cmd.ScheduledAt == dueDate.AddMinutes(-60)
            ),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_not_send_notification_when_no_dueDate()
    {
        var task = CreateTask(dueDate: null);

        var action = new CreateNotificationAction
        {
            Description = "Test"
        };

        await _executor.ExecuteAsync(task, action);

        await _mediator.DidNotReceive().Send(Arg.Any<CreateNotificationCommand>());
    }

    #endregion

    #region Calendar

    [Fact]
    public async Task Should_send_calendar_command()
    {
        var dueDate = DateTimeOffset.UtcNow;

        var task = CreateTask(dueDate: dueDate);

        var action = new CreateCalendarEventAction
        {
            DurationMinutes = 60,
            Title = "Meeting"
        };

        await _executor.ExecuteAsync(task, action);

        await _mediator.Received(1).Send(
            Arg.Is<CreateCalendarEventCommand>(cmd =>
                cmd.Title == "Meeting"
            ),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_calculate_calendar_end_time()
    {
        var start = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero);

        var task = CreateTask(dueDate: start);

        var action = new CreateCalendarEventAction
        {
            DurationMinutes = 30
        };

        await _executor.ExecuteAsync(task, action);

        await _mediator.Received().Send(
            Arg.Is<CreateCalendarEventCommand>(cmd =>
                cmd.EndTime == start.AddMinutes(30)
            ),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_not_send_calendar_when_no_dueDate()
    {
        var task = CreateTask(dueDate: null);

        var action = new CreateCalendarEventAction
        {
            DurationMinutes = 30
        };

        await _executor.ExecuteAsync(task, action);

        await _mediator.DidNotReceive().Send(Arg.Any<CreateCalendarEventCommand>());
    }

    #endregion

    #region Helpers

    private static TaskItem CreateTask(
        string title = "Test",
        TaskItemStatus status = TaskItemStatus.New,
        DateTimeOffset? dueDate = null,
        Guid? parentId = null)
    {
        return new()
        {
            Title = title,
            Status = status,
            DueDate = dueDate,
            ParentTaskId = parentId,
            OwnerId = Guid.NewGuid(),
            Owner = new User()
        };
    }

    #endregion
}
