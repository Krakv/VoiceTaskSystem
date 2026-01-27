using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.ConfirmVoiceTask;

public sealed class ConfirmVoiceTaskHandler : IRequestHandler<ConfirmVoiceTaskCommand, ConfirmVoiceTaskResponse>
{
    public Task<ConfirmVoiceTaskResponse> Handle(ConfirmVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
