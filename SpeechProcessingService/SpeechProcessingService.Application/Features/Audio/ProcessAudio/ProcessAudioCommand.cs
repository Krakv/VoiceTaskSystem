using MediatR;
using SpeechProcessingService.Application.DTOs;

namespace SpeechProcessingService.Application.Features.Audio.ProcessAudio;

public record ProcessAudioCommand(AudioFile AudioFile) : IRequest<ProcessAudioResponse>;
