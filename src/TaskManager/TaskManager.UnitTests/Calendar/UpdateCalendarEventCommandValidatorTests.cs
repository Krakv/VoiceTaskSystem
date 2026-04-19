using FluentValidation.TestHelper;
using TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;

namespace TaskManager.UnitTests.Calendar;

public class UpdateCalendarEventCommandValidatorTests
{
    private readonly UpdateCalendarEventCommandValidator _validator = new();

    private static UpdateCalendarEventCommand ValidModel() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Meeting",
        DateTimeOffset.UtcNow.AddHours(1),
        DateTimeOffset.UtcNow.AddHours(2),
        "Office",
        null,
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
    public void Should_Have_Error_When_StartTime_In_Past()
    {
        var model = ValidModel() with
        {
            StartTime = DateTimeOffset.UtcNow.AddMinutes(-5)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.StartTime);
    }

    [Fact]
    public void Should_Have_Error_When_EndTime_Before_StartTime()
    {
        var now = DateTimeOffset.UtcNow.AddHours(1);

        var model = ValidModel() with
        {
            StartTime = now,
            EndTime = now.AddMinutes(-1)
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EndTime);
    }

    [Fact]
    public void Should_Not_Have_Errors_For_Valid_Model()
    {
        var result = _validator.TestValidate(ValidModel());

        result.ShouldNotHaveAnyValidationErrors();
    }
}
