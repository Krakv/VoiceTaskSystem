using MediatR;
using TaskManager.Auth.Application.Interfaces;
using TaskManager.Auth.Infrastructure;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetAuthorizeUrl;

public sealed class GetAuthorizeUrlQueryHandler(IStateService stateService, YandexOAuthClient oAuthClient) : IRequestHandler<GetAuthorizeUrlQuery, string>
{
    private readonly IStateService _stateService = stateService;
    private readonly YandexOAuthClient _oAuthClient = oAuthClient;
    public Task<string> Handle(GetAuthorizeUrlQuery request, CancellationToken cancellationToken)
    {
        var userId = request.OwnerId;

        var state = _stateService.Generate(userId);

        var url = _oAuthClient.GetAuthorizeUrl(state);

        return url;
    }
}
