using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public sealed class GetTasksHandler(AppDbContext context, ILogger<GetTasksHandler> logger) : IRequestHandler<GetTasksQuery, GetTasksResponse>
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<GetTasksHandler> _logger = logger;

    public async Task<GetTasksResponse> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var userId = request.OwnerId;

        var query = _context.TaskItems
            .Where(x => x.OwnerId == userId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Query))
            query = query.Where(x => x.Title.Contains(request.Query));
        if (request.Status != null)
        {
            Enum.TryParse(request.Status, true, out TaskItemStatus status);
            query = query.Where(x => x.Status == status);
        }
        if (request.Priority != null)
        {
            Enum.TryParse(request.Priority, true, out TaskItemPriority priority);
            query = query.Where(x => x.Priority == priority);
        }

        var sortColumn = string.IsNullOrEmpty(request.SortBy) ? "DueDate" : request.SortBy;
        var sortOrder = request.SortOrder == "DESC" ? " descending" : " ascending";

        query = query.OrderBy($"{sortColumn} {sortOrder}");

        var limit = request.Limit ?? 20;
        var page = request.Page ?? 0;

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
