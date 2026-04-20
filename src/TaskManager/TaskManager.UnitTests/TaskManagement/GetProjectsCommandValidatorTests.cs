using FluentValidation.TestHelper;
using TaskManager.TaskManagement.Application.Features.TaskFeature.GetProjects;

namespace TaskManager.UnitTests.TaskManagement;

public class GetProjectsCommandValidatorTests
{
    private readonly GetProjectsCommandValidator _validator = new();

    private static GetProjectsCommand ValidModel() => new(
        Guid.NewGuid(),
        "search",
        0,
        10
    );

    [Fact]
    public void Should_Have_Error_When_Search_Is_Null()
    {
        var model = ValidModel() with { Search = null! };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Search);
    }

    [Fact]
    public void Should_Have_Error_When_Page_Negative()
    {
        var model = ValidModel() with { Page = -1 };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Should_Have_Error_When_PageSize_Invalid()
    {
        var model = ValidModel() with { PageSize = 0 };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
