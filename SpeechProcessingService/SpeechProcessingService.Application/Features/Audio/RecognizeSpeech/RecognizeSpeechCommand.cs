using MediatR;
using SpeechProcessingService.Application.DTOs;

namespace SpeechProcessingService.Application.Features.Audio.RecognizeSpeech;

public record RecognizeSpeechCommand(AudioFile AudioFile) : IRequest<string>;
