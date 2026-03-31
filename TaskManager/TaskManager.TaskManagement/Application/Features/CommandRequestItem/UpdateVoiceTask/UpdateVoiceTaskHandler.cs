using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.UpdateVoiceTask;

public sealed class UpdateVoiceTaskHandler : IRequestHandler<UpdateVoiceTaskCommand, UpdateVoiceTaskResponse>
{
    public Task<UpdateVoiceTaskResponse> Handle(UpdateVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
