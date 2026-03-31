using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.CreateVoiceTask;

public sealed record CreateVoiceTaskCommand(InputFile inputFile) : IRequest<CreateVoiceTaskResponse>;
