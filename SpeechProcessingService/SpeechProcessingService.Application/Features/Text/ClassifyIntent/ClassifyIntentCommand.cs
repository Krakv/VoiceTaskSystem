using MediatR;

namespace SpeechProcessingService.Application.Features.Text.ClassifyIntent;

public record ClassifyIntentCommand(string Text) : IRequest<string>;
