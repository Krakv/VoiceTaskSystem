using FluentAssertions;
using TaskManager.RulesEngine.Application.Services;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Exceptions;

namespace TaskManager.UnitTests.RuleEngine;

public class RuleSchemaValidatorTests
{
    private readonly RuleSchemaValidator _validator = new();

    #region Actions

    [Fact]
    public void Should_throw_when_actions_null()
    {
        var act = () => _validator.ValidateStructure(null, null!);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Actions не могут быть null*");
    }

    [Fact]
    public void Should_throw_when_actions_empty()
    {
        var act = () => _validator.ValidateStructure(null, []);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*хотя бы одно действие*");
    }

    [Fact]
    public void Should_throw_when_action_null()
    {
        var actions = new RuleAction[] { null! };

        var act = () => _validator.ValidateStructure(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Action не может быть null*");
    }

    [Fact]
    public void Should_throw_when_unknown_action_type()
    {
        var actions = new RuleAction[]
        {
            new FakeAction()
        };

        var act = () => _validator.ValidateStructure(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Неизвестный тип действия*");
    }

    [Fact]
    public void Should_throw_when_setfield_field_empty()
    {
        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "", Value = "test" }
        };

        var act = () => _validator.ValidateStructure(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Field обязателен*");
    }

    [Fact]
    public void Should_not_throw_when_setfield_value_empty()
    {
        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "Name", Value = "" }
        };

        var act = () => _validator.ValidateStructure(null, actions);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_throw_when_notification_description_null()
    {
        var actions = new RuleAction[]
        {
            new CreateNotificationAction { Description = null! }
        };

        var act = () => _validator.ValidateStructure(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Description обязателен*");
    }

    [Fact]
    public void Should_throw_when_calendar_duration_zero()
    {
        var actions = new RuleAction[]
        {
            new CreateCalendarEventAction { DurationMinutes = 0 }
        };

        var act = () => _validator.ValidateStructure(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*DurationMinutes обязателен*");
    }

    #endregion

    #region Conditions

    [Fact]
    public void Should_pass_when_condition_null()
    {
        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "Name", Value = "Test" }
        };

        var act = () => _validator.ValidateStructure(null, actions);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_throw_when_conditions_null()
    {
        var condition = new ConditionGroup
        {
            Conditions = null!
        };

        var actions = ValidActions();

        var act = () => _validator.ValidateStructure(condition, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Conditions не могут быть null*");
    }

    [Fact]
    public void Should_throw_when_condition_null_inside_collection()
    {
        var condition = new ConditionGroup
        {
            Conditions = [null!]
        };

        var actions = ValidActions();

        var act = () => _validator.ValidateStructure(condition, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Condition не может быть null*");
    }

    [Fact]
    public void Should_throw_when_condition_field_empty()
    {
        var condition = new ConditionGroup
        {
            Conditions =
            [
                new Condition { Field = "", Value = "test" }
            ]
        };

        var actions = ValidActions();

        var act = () => _validator.ValidateStructure(condition, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Field обязателен*");
    }

    [Fact]
    public void Should_not_throw_when_condition_value_empty()
    {
        var condition = new ConditionGroup
        {
            Conditions =
            [
                new Condition { Field = "Name", Value = "" }
            ]
        };

        var actions = ValidActions();

        var act = () => _validator.ValidateStructure(condition, actions);

        act.Should().NotThrow();
    }

    #endregion

    [Fact]
    public void Should_pass_when_valid_structure()
    {
        var condition = new ConditionGroup
        {
            Conditions =
            [
                new Condition { Field = "Name", Value = "Test" }
            ]
        };

        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "Name", Value = "NewValue" },
            new CreateNotificationAction { Description = "Test" },
            new CreateCalendarEventAction { DurationMinutes = 30 }
        };

        var act = () => _validator.ValidateStructure(condition, actions);

        act.Should().NotThrow();
    }

    private static RuleAction[] ValidActions() =>
    [
        new SetFieldAction { Field = "Name", Value = "Test" }
    ];

    private class FakeAction : RuleAction { }
}
