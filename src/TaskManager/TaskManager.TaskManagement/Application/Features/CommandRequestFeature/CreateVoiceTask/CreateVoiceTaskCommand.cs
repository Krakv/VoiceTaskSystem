using MediatR;
using TaskManager.Shared.DTOs.Requests;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.CreateVoiceTask;

public sealed record CreateVoiceTaskCommand(InputFile inputFile) : IRequest<CreateVoiceTaskResponse>;
