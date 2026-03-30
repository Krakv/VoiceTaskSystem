using MediatR;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Features.Text.ClassifyIntent;

public class ClassifyIntentCommandHandler(IIntentClassificationService service) : IRequestHandler<ClassifyIntentCommand, string>
{
    private readonly IIntentClassificationService _service = service;
    public async Task<string> Handle(ClassifyIntentCommand request, CancellationToken cancellationToken)
    {
        var res = await _service.ClassifyIntentAsync(request.Text);

        return res;
    }
}
