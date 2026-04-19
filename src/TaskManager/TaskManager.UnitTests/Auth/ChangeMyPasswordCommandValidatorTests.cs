using FluentValidation.TestHelper;
using TaskManager.Auth.Application.Features.Auth.ChangeMyPassword;

namespace TaskManager.UnitTests.Auth;

public class ChangeMyPasswordCommandValidatorTests
{
    private readonly ChangeMyPasswordCommandValidator _validator = new();

    private static ChangeMyPasswordCommand ValidModel() => new(
        Guid.NewGuid(),
        "OldPass1",
        "NewPass1"
    );

    [Fact]
    public void Should_Have_Error_When_OwnerId_Empty()
    {
        var model = ValidModel() with { OwnerId = Guid.Empty };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.OwnerId);
    }

    [Fact]
    public void Should_Have_Error_When_CurrentPassword_Empty()
    {
        var model = ValidModel() with { CurrentPassword = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Fact]
    public void Should_Have_Error_When_CurrentPassword_Too_Short()
    {
        var model = ValidModel() with { CurrentPassword = "Ab1" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_Empty()
    {
        var model = ValidModel() with { NewPassword = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_Too_Short()
    {
        var model = ValidModel() with { NewPassword = "Ab1" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_No_Uppercase()
    {
        var model = ValidModel() with { NewPassword = "lowercase1" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_No_Lowercase()
    {
        var model = ValidModel() with { NewPassword = "UPPERCASE1" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_No_Digit()
    {
        var model = ValidModel() with { NewPassword = "NoDigits" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_Equals_Current()
    {
        var model = ValidModel() with
        {
            CurrentPassword = "SamePass1",
            NewPassword = "SamePass1"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
