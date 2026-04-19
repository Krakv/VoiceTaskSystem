using FluentValidation.TestHelper;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.UnitTests.Notifications;

public class CreateNotificationCommandValidatorTests
{
    private readonly CreateNotificationCommandValidator _validator = new();

    private static CreateNotificationCommand ValidModel() => new(
        Guid.NewGuid(),
        NotificationServiceType.Email,
        "Test notification",
        DateTimeOffset.UtcNow.AddMinutes(10),
        null
    );

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var model = ValidModel() with { Description = "" };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Too_Long()
    {
        var model = ValidModel() with
        {
            Description = new string('a', 256)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_ScheduledAt_In_Past()
    {
        var model = ValidModel() with
        {
            ScheduledAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
