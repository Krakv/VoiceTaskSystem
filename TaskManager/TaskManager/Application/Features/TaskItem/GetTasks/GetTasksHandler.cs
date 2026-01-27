using MediatR;
using TaskManager.Infrastructure.Repository;
using TaskManager.Middleware;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed class GetTasksHandler(AppDbContext context, ICurrentUser user) : IRequestHandler<GetTasksQuery, GetTasksResponse>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

    public async Task<GetTasksResponse> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.UserId;

        var query = _context.TaskItems
            .Where(x => x.OwnerId == userId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status)) query = query.Where(x => x.Status == request.Status);
        if (!string.IsNullOrEmpty(request.Priority)) query = query.Where(x => x.Priority == request.Priority);

        var sortColumn = string.IsNullOrEmpty(request.SortBy) ? "DueDate" : request.SortBy;
        var sortOrder = request.SordOrder == "DESC" ? " descending" : "";
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
                item.Status, item.Priority, item.Tags, item.DueDate,
                item.CreatedAt, item.UpdatedAt, item.ParentTaskId
            )).ToListAsync(cancellationToken);

        return new GetTasksResponse(tasks, pagination);
    }
}
