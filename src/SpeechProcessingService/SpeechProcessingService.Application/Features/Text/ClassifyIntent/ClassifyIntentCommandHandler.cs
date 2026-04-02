using MediatR;
using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Features.Text.ClassifyIntent;

public class ClassifyIntentCommandHandler(IIntentClassificationService service) : IRequestHandler<ClassifyIntentCommand, CommandIntent>
{
    private readonly IIntentClassificationService _service = service;
    public async Task<CommandIntent> Handle(ClassifyIntentCommand request, CancellationToken cancellationToken)
    {
        var res = await _service.ClassifyIntentAsync(request.Text);

        return res;
    }
}
