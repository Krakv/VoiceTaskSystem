using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public sealed class GetVoiceTaskStatusHandler : IRequestHandler<GetVoiceTaskStatusQuery, GetVoiceTaskStatusResponse>
{
    public Task<GetVoiceTaskStatusResponse> Handle(GetVoiceTaskStatusQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
