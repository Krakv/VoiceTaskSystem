using MediatR;

namespace SpeechProcessingService.Application.Features.Text.ExtractEntities;

public record ExtractEntitiesCommand(string Text) : IRequest<string>;
