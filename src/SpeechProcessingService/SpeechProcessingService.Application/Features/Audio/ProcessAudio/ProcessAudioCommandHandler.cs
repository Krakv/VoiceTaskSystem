using MediatR;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Features.Audio.ProcessAudio;

public class ProcessAudioCommandHandler(ISpeechProcessingService speechProcessingService) : IRequestHandler<ProcessAudioCommand, ProcessAudioResponse>
{
    private readonly ISpeechProcessingService _speechProcessingService = speechProcessingService;

    public async Task<ProcessAudioResponse> Handle(ProcessAudioCommand request, CancellationToken cancellationToken)
    {
        var result = await _speechProcessingService.ProcessCommandAsync(request.CommandRequestId, request.AudioFile, request.UserTimeZone);

        return new ProcessAudioResponse(result.CommandId, result.TextRecognized, result.CommandIntent, result.Entities);
    }
}
