using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.TaskManagement.Interfaces;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetProjects;

public class GetProjectsCommandHandler(AppDbContext context, ICurrentUser user, ILogger<GetProjectsCommandHandler> logger) : IRequestHandler<GetProjectsCommand, GetProjectsResponse>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<GetProjectsCommandHandler> _logger = logger;
    public async Task<GetProjectsResponse> Handle(GetProjectsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Project list has been requested");
        
        var projects = await _context.TaskItems
            .Where(t => t.OwnerId == _user.UserId 
                && !string.IsNullOrEmpty(t.ProjectName)
                && t.ProjectName.Contains(request.Search)
                )
            .Select(x => x.ProjectName)
            .Distinct()
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new GetProjectsResponse(projects, projects.Count);
    }
}
