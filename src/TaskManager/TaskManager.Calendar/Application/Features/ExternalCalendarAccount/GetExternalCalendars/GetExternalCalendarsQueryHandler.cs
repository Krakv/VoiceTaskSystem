using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetExternalCalendars;

public sealed class GetExternalCalendarsQueryHandler(AppDbContext context, ICurrentUser currentUser): IRequestHandler<GetExternalCalendarsQuery, List<ExternalCalendarAccountDto>>
{
    public async Task<List<ExternalCalendarAccountDto>> Handle(GetExternalCalendarsQuery request,CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        return await context.ExternalCalendarAccount
            .Where(x => x.OwnerId == userId)
            .Select(x => new ExternalCalendarAccountDto
            (
                x.ExternalCalendarAccountId,
                x.BaseUrl
            ))
            .ToListAsync(cancellationToken);
    }
}
