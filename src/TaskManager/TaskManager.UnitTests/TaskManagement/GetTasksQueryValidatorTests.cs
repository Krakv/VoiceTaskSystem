using FluentValidation.TestHelper;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

namespace TaskManager.UnitTests.TaskManagement;

public class GetTasksQueryValidatorTests
{
    private readonly GetTasksQueryValidator _validator = new();

    private static GetTasksQuery ValidModel() => new(
        Guid.NewGuid(),
        "query",
        "inProgress",
        "high",
        "CreatedAt",
        "ASC",
        10,
        0
    );

    [Fact]
    public void Should_Have_Error_When_Query_Too_Long()
    {
        var model = ValidModel() with
        {
            Query = new string('a', 201)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Query);
    }

    [Fact]
    public void Should_Have_Error_When_SortOrder_Invalid()
    {
        var model = ValidModel() with
        {
            SortOrder = "INVALID"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SortOrder);
    }

    [Fact]
    public void Should_Have_Error_When_SortBy_Invalid()
    {
        var model = ValidModel() with
        {
            SortBy = "BadColumn"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
