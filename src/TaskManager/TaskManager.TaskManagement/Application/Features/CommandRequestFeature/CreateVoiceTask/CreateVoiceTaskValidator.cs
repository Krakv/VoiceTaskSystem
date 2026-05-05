using FluentValidation;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.CreateVoiceTask;

public sealed class CreateVoiceTaskValidator : AbstractValidator<CreateVoiceTaskCommand>
{
    private readonly string[] allowedTypes = ["audio/ogg", "audio/mpeg"];

    public CreateVoiceTaskValidator()
    {
        RuleFor(x => x.InputFile)
            .NotEmpty()
            .NotNull()
            .WithErrorCode("INVALID_AUDIO_FILE");

    }
}
