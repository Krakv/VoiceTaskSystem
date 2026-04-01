using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.ConfirmVoiceTask;

public sealed class ConfirmVoiceTaskHandler : IRequestHandler<ConfirmVoiceTaskCommand, ConfirmVoiceTaskResponse>
{
    public Task<ConfirmVoiceTaskResponse> Handle(ConfirmVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
