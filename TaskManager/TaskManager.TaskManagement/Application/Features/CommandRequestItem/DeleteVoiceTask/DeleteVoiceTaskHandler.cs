using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.DeleteVoiceTask;

public sealed class DeleteVoiceTaskHandler : IRequestHandler<DeleteVoiceTaskCommand, DeleteVoiceTaskResponse>
{
    public Task<DeleteVoiceTaskResponse> Handle(DeleteVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
