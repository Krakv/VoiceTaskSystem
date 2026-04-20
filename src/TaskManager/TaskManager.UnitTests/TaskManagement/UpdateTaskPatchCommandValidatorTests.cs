using FluentValidation.TestHelper;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

namespace TaskManager.UnitTests.TaskManagement;

public class UpdateTaskPatchCommandValidatorTests
{
    private readonly UpdateTaskPatchCommandValidator _validator = new();

    private static UpdateTaskPatchCommand ValidModel() => new(
        Guid.NewGuid(),
        Guid.NewGuid()
    );

    [Fact]
    public void Should_Have_Error_When_ProjectName_Too_Long()
    {
        var model = ValidModel() with
        {
            ProjectName = new string('a', 101)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProjectName);
    }

    [Fact]
    public void Should_Have_Error_When_Invalid_Guid()
    {
        var model = ValidModel() with
        {
            ParentTaskId = "invalid-guid"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ParentTaskId);
    }
}
