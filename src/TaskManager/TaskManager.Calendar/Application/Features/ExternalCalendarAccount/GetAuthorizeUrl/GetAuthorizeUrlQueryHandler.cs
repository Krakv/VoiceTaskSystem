using MediatR;
using TaskManager.Auth.Application.Interfaces;
using TaskManager.Auth.Infrastructure;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetAuthorizeUrl;

public sealed class GetAuthorizeUrlQueryHandler(ICurrentUser user, IStateService stateService, YandexOAuthClient oAuthClient) : IRequestHandler<GetAuthorizeUrlQuery, string>
{
    private readonly ICurrentUser _user = user;
    private readonly IStateService _stateService = stateService;
    private readonly YandexOAuthClient _oAuthClient = oAuthClient;
    public Task<string> Handle(GetAuthorizeUrlQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.UserId;

        var state = _stateService.Generate(userId);

        var url = _oAuthClient.GetAuthorizeUrl(state);

        return url;
    }
}
