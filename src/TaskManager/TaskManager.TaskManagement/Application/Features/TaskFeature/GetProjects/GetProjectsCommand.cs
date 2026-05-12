using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetProjects;

public record GetProjectsCommand(Guid OwnerId, string ProjectName, int Page = 0, int PageSize = 5) : IRequest<GetProjectsResponse>;
