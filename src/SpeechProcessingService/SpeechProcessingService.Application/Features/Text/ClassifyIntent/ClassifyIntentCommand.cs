using MediatR;
using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Application.Features.Text.ClassifyIntent;

public record ClassifyIntentCommand(string Text) : IRequest<CommandIntent>;
