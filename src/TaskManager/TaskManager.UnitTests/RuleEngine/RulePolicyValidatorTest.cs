using FluentAssertions;
using TaskManager.RulesEngine.Application.Services;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;

namespace TaskManager.UnitTests.RuleEngine;

public class RulePolicyValidatorTests
{
    private readonly RulePolicyValidator _validator = new();

    #region SetField policy

    [Fact]
    public void Should_throw_when_setfield_and_event_task_overdue()
    {
        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "Title", Value = "Test" }
        };

        var act = () => _validator.Validate(RuleEvent.TaskOverdue, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Нельзя изменять поля при просрочке задачи*");
    }

    [Fact]
    public void Should_pass_when_setfield_and_event_not_overdue()
    {
        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "Title", Value = "Test" }
        };

        var act = () => _validator.Validate(RuleEvent.TaskCreated, actions);

        act.Should().NotThrow();
    }

    #endregion

    #region Calendar policy

    [Fact]
    public void Should_throw_when_calendar_duration_more_than_day()
    {
        var actions = new RuleAction[]
        {
            new CreateCalendarEventAction { DurationMinutes = 1441 }
        };

        var act = () => _validator.Validate(RuleEvent.TaskCreated, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Событие не может длиться больше суток*");
    }

    [Fact]
    public void Should_pass_when_calendar_duration_equal_day()
    {
        var actions = new RuleAction[]
        {
            new CreateCalendarEventAction { DurationMinutes = 1440 }
        };

        var act = () => _validator.Validate(RuleEvent.TaskCreated, actions);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_pass_when_calendar_duration_less_than_day()
    {
        var actions = new RuleAction[]
        {
            new CreateCalendarEventAction { DurationMinutes = 60 }
        };

        var act = () => _validator.Validate(RuleEvent.TaskCreated, actions);

        act.Should().NotThrow();
    }

    #endregion

    #region Notification policy

    [Fact]
    public void Should_pass_for_notification_any_event()
    {
        var actions = new RuleAction[]
        {
            new CreateNotificationAction { Description = "Test" }
        };

        var act = () => _validator.Validate(RuleEvent.TaskOverdue, actions);

        act.Should().NotThrow();
    }

    #endregion

    #region Multiple actions

    [Fact]
    public void Should_throw_if_any_action_violates_policy()
    {
        var actions = new RuleAction[]
        {
            new CreateNotificationAction { Description = "ok" },
            new SetFieldAction { Field = "Title", Value = "Test" } // forbidden
        };

        var act = () => _validator.Validate(RuleEvent.TaskOverdue, actions);

        act.Should().Throw<ValidationAppException>();
    }

    [Fact]
    public void Should_pass_when_all_actions_valid()
    {
        var actions = new RuleAction[]
        {
            new CreateNotificationAction { Description = "ok" },
            new CreateCalendarEventAction { DurationMinutes = 30 }
        };

        var act = () => _validator.Validate(RuleEvent.TaskCreated, actions);

        act.Should().NotThrow();
    }

    #endregion
}
