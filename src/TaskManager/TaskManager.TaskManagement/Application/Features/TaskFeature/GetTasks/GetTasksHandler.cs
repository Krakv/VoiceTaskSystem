using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using TaskManager.Repository.Context;
using TaskManager.TaskManagement.Interfaces;
using TaskManager.Shared.Domain.Entities.Enum;
using Microsoft.Extensions.Logging;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public sealed class GetTasksHandler(AppDbContext context, ICurrentUser user, ILogger<GetTasksHandler> logger) : IRequestHandler<GetTasksQuery, GetTasksResponse>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<GetTasksHandler> _logger = logger;

    public async Task<GetTasksResponse> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.UserId;

        var query = _context.TaskItems
            .Where(x => x.OwnerId == userId)
            .AsQueryable();
        if (!string.IsNullOrEmpty(request.Query))
            query = query.Where(x => x.Title.Contains(request.Query));
        if (!string.IsNullOrEmpty(request.Status) 
            && Enum.TryParse<TaskItemStatus>(request.Status, ignoreCase: true, out var statusParsed)) query = query.Where(x => x.Status == statusParsed);
        if (!string.IsNullOrEmpty(request.Priority) 
            && Enum.TryParse<TaskItemPriority>(request.Priority, ignoreCase: true, out var priorityParsed)) query = query.Where(x => x.Priority == priorityParsed);

        var sortColumn = string.IsNullOrEmpty(request.SortBy) ? "DueDate" : request.SortBy;
        var sortOrder = request.SortOrder == "DESC" ? " descending" : "";
        query = query.OrderBy(sortColumn + sortOrder);

        if(!Int32.TryParse(request.Limit, out int limit)) limit = 20;
        if (limit <= 0) limit = 20;

        if(!Int32.TryParse(request.Page, out int page)) page = 0;
        if (page < 0) page = 0;

        var total = await query.CountAsync(cancellationToken);
        var pages = (total + limit - 1) / limit;
        var pagination = new Pagination(limit, page, total, pages);

        query = query.Skip(page * limit).Take(limit);

        var tasks = await query.Select(item => new TaskListElement(
                item.TaskId, item.ProjectName, item.Title, item.Description,
                item.Status, item.Priority, item.DueDate,
                item.CreatedAt, item.UpdatedAt, item.ParentTaskId
            )).ToListAsync(cancellationToken);

        _logger.LogDebug("Task list has been requested");
        return new GetTasksResponse(tasks, pagination);
    }
}
