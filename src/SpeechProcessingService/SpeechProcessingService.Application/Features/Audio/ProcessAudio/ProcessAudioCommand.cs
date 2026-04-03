using MediatR;
using SpeechProcessingService.Application.DTOs;

namespace SpeechProcessingService.Application.Features.Audio.ProcessAudio;

public record ProcessAudioCommand(Guid CommandRequestId, AudioFile AudioFile, TimeZoneInfo UserTimeZone) : IRequest<ProcessAudioResponse>;
