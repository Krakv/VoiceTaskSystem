using MediatR;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Features.Audio.RecognizeSpeech;

public class RecognizeSpeechCommandHandler(IAsrService asrService) : IRequestHandler<RecognizeSpeechCommand, string>
{
    private readonly IAsrService _asrService = asrService;

    public async Task<string> Handle(RecognizeSpeechCommand request, CancellationToken cancellationToken)
    {
        var result = await _asrService.RecognizeSpeechAsync(request.AudioFile);

        return result;
    }
}
