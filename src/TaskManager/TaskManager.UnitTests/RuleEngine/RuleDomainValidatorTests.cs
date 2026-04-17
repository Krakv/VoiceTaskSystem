using FluentAssertions;
using TaskManager.RulesEngine.Application.Services;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Exceptions;

namespace TaskManager.UnitTests.RuleEngine;

public class RuleDomainValidatorTests
{
    private readonly RuleDomainValidator _validator = new();

    #region Field validation

    [Fact]
    public void Should_throw_when_field_not_exists()
    {
        var condition = Condition("UnknownField", "test");

        var act = () => _validator.Validate(condition, ValidActions());

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Поле не существует*");
    }

    [Fact]
    public void Should_pass_when_field_exists_case_insensitive()
    {
        var condition = Condition("title", "Test");

        var act = () => _validator.Validate(condition, ValidActions());

        act.Should().NotThrow();
    }

    #endregion

    #region Value type validation

    [Fact]
    public void Should_throw_when_invalid_guid()
    {
        var condition = Condition("ParentTaskId", "not-a-guid");

        var act = () => _validator.Validate(condition, ValidActions());

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Неверное значение*");
    }

    [Fact]
    public void Should_throw_when_invalid_datetime()
    {
        var condition = Condition("DueDate", "not-a-date");

        var act = () => _validator.Validate(condition, ValidActions());

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Неверное значение*");
    }

    [Fact]
    public void Should_pass_when_valid_datetime()
    {
        var condition = Condition("DueDate", "2024-01-01");

        var act = () => _validator.Validate(condition, ValidActions());

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_pass_when_valid_enum()
    {
        var condition = Condition("Status", "New");

        var act = () => _validator.Validate(condition, ValidActions());

        act.Should().NotThrow();
    }

    #endregion

    #region Operator validation

    [Fact]
    public void Should_throw_when_invalid_operator()
    {
        var condition = new ConditionGroup
        {
            Conditions =
            [
                new Condition
                {
                    Field = "Title",
                    Value = "test",
                    Operator = (ComparisonOperator)999
                }
            ]
        };

        var act = () => _validator.Validate(condition, ValidActions());

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Недопустимый оператор*");
    }

    #endregion

    #region SetFieldAction

    [Fact]
    public void Should_throw_when_setfield_invalid_field()
    {
        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "Unknown", Value = "test" }
        };

        var act = () => _validator.Validate(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Поле не существует*");
    }

    [Fact]
    public void Should_throw_when_setfield_invalid_value()
    {
        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "ParentTaskId", Value = "bad-guid" }
        };

        var act = () => _validator.Validate(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Неверное значение*");
    }

    #endregion

    #region CreateCalendarEventAction

    [Fact]
    public void Should_throw_when_duration_less_or_equal_zero()
    {
        var actions = new RuleAction[]
        {
            new CreateCalendarEventAction { DurationMinutes = 0 }
        };

        var act = () => _validator.Validate(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*DurationMinutes должен быть > 0*");
    }

    [Fact]
    public void Should_throw_when_external_account_invalid_guid()
    {
        var actions = new RuleAction[]
        {
            new CreateCalendarEventAction
            {
                DurationMinutes = 10,
                ExternalAccountId = "bad-guid"
            }
        };

        var act = () => _validator.Validate(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*ExternalAccountId должен быть валидным GUID*");
    }

    [Fact]
    public void Should_pass_when_external_account_valid_guid()
    {
        var actions = new RuleAction[]
        {
            new CreateCalendarEventAction
            {
                DurationMinutes = 10,
                ExternalAccountId = Guid.NewGuid().ToString()
            }
        };

        var act = () => _validator.Validate(null, actions);

        act.Should().NotThrow();
    }

    #endregion

    #region CreateNotificationAction

    [Fact]
    public void Should_throw_when_description_empty()
    {
        var actions = new RuleAction[]
        {
            new CreateNotificationAction
            {
                Description = ""
            }
        };

        var act = () => _validator.Validate(null, actions);

        act.Should().Throw<ValidationAppException>()
            .WithMessage("*Description не может быть пустым*");
    }

    #endregion

    #region Happy path

    [Fact]
    public void Should_pass_when_valid_input()
    {
        var condition = Condition("Title", "Test");

        var actions = new RuleAction[]
        {
            new SetFieldAction { Field = "Title", Value = "New" },
            new CreateCalendarEventAction { DurationMinutes = 30 }
        };

        var act = () => _validator.Validate(condition, actions);

        act.Should().NotThrow();
    }

    #endregion

    #region Helpers

    private static ConditionGroup Condition(string field, string value) =>
        new()
        {
            Conditions =
            [
                new Condition
                {
                    Field = field,
                    Value = value,
                    Operator = ComparisonOperator.eq
                }
            ]
        };

    private static RuleAction[] ValidActions() =>
    [
        new SetFieldAction { Field = "Title", Value = "Test" }
    ];

    #endregion
}