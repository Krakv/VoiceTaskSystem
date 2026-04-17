using FluentAssertions;
using TaskManager.RulesEngine.Application.Services;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.UnitTests.RuleEngine;

public class RuleEvaluatorTests
{
    private readonly RuleEvaluator _evaluator = new();

    #region Null / base cases

    [Fact]
    public void Should_return_true_when_group_null()
    {
        var task = CreateTask();

        var result = _evaluator.Evaluate(task, null);

        result.Should().BeTrue();
    }

    #endregion

    #region EQ / NEQ

    [Fact]
    public void Should_return_true_for_eq()
    {
        var task = CreateTask(title: "Test");

        var result = _evaluator.Evaluate(task,
            Condition("Title", "Test", ComparisonOperator.eq));

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_return_true_for_neq()
    {
        var task = CreateTask(title: "Test");

        var result = _evaluator.Evaluate(task,
            Condition("Title", "Other", ComparisonOperator.neq));

        result.Should().BeTrue();
    }

    #endregion

    #region GT / LT (int)

    [Fact]
    public void Should_return_true_for_gt_int()
    {
        var task = CreateTask(priority: TaskItemPriority.High);

        var result = _evaluator.Evaluate(task,
            Condition("Priority", "medium", ComparisonOperator.gt));

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_return_true_for_lt_int()
    {
        var task = CreateTask(priority: TaskItemPriority.Low);

        var result = _evaluator.Evaluate(task,
            Condition("Priority", "medium", ComparisonOperator.lt));

        result.Should().BeTrue();
    }

    #endregion

    #region DateTimeOffset (NEW логика)

    [Fact]
    public void Should_return_true_for_datetime_gt()
    {
        var task = CreateTask(dueDate: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var result = _evaluator.Evaluate(task,
            Condition("DueDate", "2024-01-01", ComparisonOperator.gt));

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_return_false_when_invalid_datetime()
    {
        var task = CreateTask(dueDate: DateTimeOffset.UtcNow);

        var result = _evaluator.Evaluate(task,
            Condition("DueDate", "not-a-date", ComparisonOperator.eq));

        result.Should().BeFalse();
    }

    #endregion

    #region Enum parsing

    [Fact]
    public void Should_parse_enum_case_insensitive()
    {
        var task = CreateTask(status: TaskItemStatus.InProgress);

        var result = _evaluator.Evaluate(task,
            Condition("Status", "inprogress", ComparisonOperator.eq));

        result.Should().BeTrue();
    }

    #endregion

    #region Guid

    [Fact]
    public void Should_return_true_for_guid_eq()
    {
        var id = Guid.NewGuid();
        var task = CreateTask(parentId: id);

        var result = _evaluator.Evaluate(task,
            Condition("ParentTaskId", id.ToString(), ComparisonOperator.eq));

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_return_false_for_invalid_guid()
    {
        var task = CreateTask(parentId: Guid.NewGuid());

        var result = _evaluator.Evaluate(task,
            Condition("ParentTaskId", "bad-guid", ComparisonOperator.eq));

        result.Should().BeFalse();
    }

    #endregion

    #region AND / OR

    [Fact]
    public void Should_return_true_for_and()
    {
        var task = CreateTask(title: "Test", priority: TaskItemPriority.High);

        var group = new ConditionGroup
        {
            Operator = LogicalOperator.AND,
            Conditions =
            [
                new Condition { Field = "Title", Value = "Test", Operator = ComparisonOperator.eq },
                new Condition { Field = "Priority", Value = "medium", Operator = ComparisonOperator.gt }
            ]
        };

        var result = _evaluator.Evaluate(task, group);

        result.Should().BeTrue();
    }

    [Fact]
    public void Should_return_false_for_and_when_one_fails()
    {
        var task = CreateTask(title: "Test", priority: TaskItemPriority.Low);

        var group = new ConditionGroup
        {
            Operator = LogicalOperator.AND,
            Conditions =
            [
                new Condition { Field = "Title", Value = "Test", Operator = ComparisonOperator.eq },
                new Condition { Field = "Priority", Value = "medium", Operator = ComparisonOperator.gt }
            ]
        };

        var result = _evaluator.Evaluate(task, group);

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_return_true_for_or()
    {
        var task = CreateTask(title: "Test1", priority: TaskItemPriority.Medium);

        var group = new ConditionGroup
        {
            Operator = LogicalOperator.OR,
            Conditions =
            [
                new Condition { Field = "Title", Value = "Test", Operator = ComparisonOperator.eq },
                new Condition { Field = "Priority", Value = "low", Operator = ComparisonOperator.gt }
            ]
        };

        var result = _evaluator.Evaluate(task, group);

        result.Should().BeTrue();
    }

    #endregion

    #region Edge cases (важно)

    [Fact]
    public void Should_return_false_when_field_not_exists()
    {
        var task = CreateTask();

        var result = _evaluator.Evaluate(task,
            Condition("UnknownField", "test", ComparisonOperator.eq));

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_return_false_when_task_value_null()
    {
        var task = CreateTask(description: null);

        var result = _evaluator.Evaluate(task,
            Condition("Description", "test", ComparisonOperator.eq));

        result.Should().BeFalse();
    }

    [Fact]
    public void Should_return_false_when_compare_not_supported()
    {
        var task = CreateTask(title: "Test");

        var result = _evaluator.Evaluate(task,
            Condition("Title", "Another", ComparisonOperator.gt));

        result.Should().BeFalse();
    }

    #endregion

    #region Helpers

    private static ConditionGroup Condition(string field, string value, ComparisonOperator op) =>
        new()
        {
            Operator = LogicalOperator.AND,
            Conditions =
            [
                new Condition
                {
                    Field = field,
                    Value = value,
                    Operator = op
                }
            ]
        };

    private static TaskItem CreateTask(
        string title = "Test",
        TaskItemStatus status = TaskItemStatus.New,
        TaskItemPriority priority = TaskItemPriority.Low,
        string? description = "Desc",
        DateTimeOffset? dueDate = null,
        Guid? parentId = null)
    {
        return new TaskItem
        {
            Title = title,
            Status = status,
            Priority = priority,
            Description = description,
            DueDate = dueDate,
            ParentTaskId = parentId,
            OwnerId = Guid.NewGuid(),
            Owner = new User()
        };
    }

    #endregion
}