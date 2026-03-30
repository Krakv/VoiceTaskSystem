using MediatR;

namespace SpeechProcessingService.Application.Features.Audio.ProcessAudio;

public record ProcessAudioCommand(IFormFile AudioFile) : IRequest<ProcessAudioResponse>;
