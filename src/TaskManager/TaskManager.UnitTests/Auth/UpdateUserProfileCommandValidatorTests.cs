using FluentValidation.TestHelper;
using TaskManager.Auth.Application.Features.Auth.UpdateUserProfile;

namespace TaskManager.UnitTests.Auth;

public class UpdateUserProfileCommandValidatorTests
{
    private readonly UpdateUserProfileCommandValidator _validator = new();

    private static UpdateUserProfileCommand ValidModel() => new(
        Guid.NewGuid(),
        "John Doe",
        "john@example.com"
    );

    [Fact]
    public void Should_Have_Error_When_OwnerId_Empty()
    {
        var model = ValidModel() with { OwnerId = Guid.Empty };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.OwnerId);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var model = ValidModel() with
        {
            Name = new string('a', 101)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Invalid()
    {
        var model = ValidModel() with
        {
            Email = "invalid-email"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Null()
    {
        var model = ValidModel() with { Email = null };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Null()
    {
        var model = ValidModel() with { Name = null };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
