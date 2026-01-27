using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.DeleteVoiceTask;

public sealed class DeleteVoiceTaskHandler : IRequestHandler<DeleteVoiceTaskCommand, DeleteVoiceTaskResponse>
{
    public Task<DeleteVoiceTaskResponse> Handle(DeleteVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
