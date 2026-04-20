using FluentValidation.TestHelper;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

namespace TaskManager.UnitTests.TaskManagement;

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator = new();

    private static CreateTaskCommand ValidModel() => new(
        Guid.NewGuid(),
        "Project",
        "Title",
        "Description",
        TaskItemStatus.InProgress,
        TaskItemPriority.Medium,
        DateTimeOffset.UtcNow.AddDays(1),
        null
    );

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var model = ValidModel() with { Title = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Too_Long()
    {
        var model = ValidModel() with
        {
            Title = new string('a', 101)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_DueDate_In_Past()
    {
        var model = ValidModel() with
        {
            DueDate = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void Should_Have_Error_When_Status_Invalid()
    {
        var model = ValidModel() with
        {
            Status = (TaskItemStatus)999
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_Have_Error_When_Priority_Invalid()
    {
        var model = ValidModel() with
        {
            Priority = (TaskItemPriority)999
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
