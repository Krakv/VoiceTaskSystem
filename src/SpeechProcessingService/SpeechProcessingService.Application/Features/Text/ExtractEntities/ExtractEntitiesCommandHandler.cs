using MediatR;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Features.Text.ExtractEntities;

public class ExtractEntitiesCommandHandler(IEntityExtractionService entityExtractionService) : IRequestHandler<ExtractEntitiesCommand, Dictionary<string, string>>
{
    private readonly IEntityExtractionService _entityExtractionService = entityExtractionService;

    public async Task<Dictionary<string, string>> Handle(ExtractEntitiesCommand request, CancellationToken cancellationToken)
    {
        var result = await _entityExtractionService.ExtractEntitiesAsync(request.Text);
        return result;
    }
}
