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
            .Where(t => t.OwnerId == _user.UserId && !string.IsNullOrEmpty(t.ProjectName))
            .Select(t => new
            {
                t.ProjectName,
                Score = FuzzySharp.Fuzz.TokenSetRatio(request.Search, t.ProjectName)
            })
            .OrderByDescending(x => x.Score)
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .Select(x => x.ProjectName)
            .ToListAsync(cancellationToken);

        return new GetProjectsResponse(projects, projects.Count);
    }
}
