using FluentValidation.TestHelper;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;

namespace TaskManager.UnitTests.TaskManagement;

public class UpdateTaskCommandValidatorTests
{
    private readonly UpdateTaskCommandValidator _validator = new();

    private static UpdateTaskCommand ValidModel() => new(
        Guid.NewGuid(),
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
    public void Should_Have_Error_When_DueDate_In_Past()
    {
        var model = ValidModel() with
        {
            DueDate = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }
}
