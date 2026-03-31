using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.CreateVoiceTask;

public sealed class CreateVoiceTaskHandler : IRequestHandler<CreateVoiceTaskCommand, CreateVoiceTaskResponse>
{
    public Task<CreateVoiceTaskResponse> Handle(CreateVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
