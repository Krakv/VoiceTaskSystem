using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;

public sealed class UpdateVoiceTaskHandler : IRequestHandler<UpdateVoiceTaskCommand, UpdateVoiceTaskResponse>
{
    public Task<UpdateVoiceTaskResponse> Handle(UpdateVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
