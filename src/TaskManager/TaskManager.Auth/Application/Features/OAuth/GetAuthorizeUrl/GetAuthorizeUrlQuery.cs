using MediatR;

namespace TaskManager.Auth.Application.Features.OAuth.GetAuthorizeUrl;

public sealed record GetAuthorizeUrlQuery() : IRequest<string>;
