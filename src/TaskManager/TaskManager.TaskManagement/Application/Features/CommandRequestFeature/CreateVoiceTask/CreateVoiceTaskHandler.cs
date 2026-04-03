using MediatR;
using TaskManager.Shared.DTOs.Requests;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Repository.Context;
using TaskManager.TaskManagement.Interfaces;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DTOs;
using TaskManager.TaskManagement.Application.Services.Interfaces;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.CreateVoiceTask;

public sealed class CreateVoiceTaskHandler(AppDbContext dbContext, ICurrentUser user, IBackgroundTaskQueue queue) : IRequestHandler<CreateVoiceTaskCommand, CreateVoiceTaskResponse>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ICurrentUser _user = user;
    private readonly IBackgroundTaskQueue _queue = queue;

    public async Task<CreateVoiceTaskResponse> Handle(CreateVoiceTaskCommand request, CancellationToken cancellationToken)
    {
        var commandRequest = new CommandRequestItem
        {
            OwnerId = _user.UserId,
        };

        _dbContext.CommandRequestItem.Add(commandRequest);

        var voiceCommandDto = new VoiceCommandCreationRequestedDto
        {
            UserId = _user.UserId,
            VoiceCommandRequest = new VoiceCommandRequest
            (
                commandRequest.CommandRequestId,
                request.inputFile
            )
        };

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _queue.EnqueueAsync(voiceCommandDto);

        return await Task.FromResult(new CreateVoiceTaskResponse
        (
            commandRequest.CommandRequestId,
            "Voice task creation requested successfully."
        ));
    }
}
