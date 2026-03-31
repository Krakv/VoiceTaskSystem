using FluentValidation;

namespace TaskManager.Application.Features.CommandRequestItem.CreateVoiceTask;

public sealed class CreateVoiceTaskValidator : AbstractValidator<CreateVoiceTaskCommand>
{
    private readonly string[] allowedTypes = ["audio/ogg", "audio/mpeg"];

    public CreateVoiceTaskValidator()
    {
        RuleFor(x => x.inputFile)
            .NotEmpty()
            .NotNull()
            .WithErrorCode("INVALID_AUDIO_FILE")
            .DependentRules(() =>
            {
                RuleFor(x => x.inputFile.ContentType)
                    .NotEmpty()
                    .Must(x => allowedTypes.Contains(x))
                    .WithErrorCode("UNSUPPORTED_MEDIA_TYPE");
            });

    }
}
