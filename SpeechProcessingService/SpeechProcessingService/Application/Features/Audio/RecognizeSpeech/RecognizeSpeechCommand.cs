using MediatR;

namespace SpeechProcessingService.Application.Features.Audio.RecognizeSpeech;

public record RecognizeSpeechCommand(IFormFile AudioFile) : IRequest<string>;
