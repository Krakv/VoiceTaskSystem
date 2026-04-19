using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetExternalCalendars;

public sealed class GetExternalCalendarsQueryHandler(AppDbContext context): IRequestHandler<GetExternalCalendarsQuery, List<ExternalCalendarAccountDto>>
{
    private readonly AppDbContext _context = context;
    public async Task<List<ExternalCalendarAccountDto>> Handle(GetExternalCalendarsQuery request,CancellationToken cancellationToken)
    {
        var userId = request.OwnerId;

        return await _context.ExternalCalendarAccount
            .Where(x => x.OwnerId == userId)
            .Select(x => new ExternalCalendarAccountDto
            (
                x.ExternalCalendarAccountId,
                x.BaseUrl
            ))
            .ToListAsync(cancellationToken);
    }
}
