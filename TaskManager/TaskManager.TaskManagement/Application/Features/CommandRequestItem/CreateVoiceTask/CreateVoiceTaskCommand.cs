using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.CreateVoiceTask;

public sealed record CreateVoiceTaskCommand(InputFile inputFile) : IRequest<CreateVoiceTaskResponse>;
