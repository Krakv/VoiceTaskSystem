using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DTOs;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;
using TaskManager.TaskManagement.Application.Services.Interfaces;

namespace TaskManager.TaskManagement.Application.Services.VoiceProcessing;

public class VoiceProcessingHandler(
    AppDbContext db, 
    ISpeechProcessingClient speechClient, 
    ILogger<VoiceProcessingHandler> logger
    )
{
    private readonly AppDbContext _db = db;
    private readonly ISpeechProcessingClient _speechClient = speechClient;
    private readonly ILogger<VoiceProcessingHandler> _logger = logger;

    public async Task Handle(VoiceCommandCreationRequestedDto request)
    {
        _logger.LogInformation("Started to process voice command: {CommandRequestId}", request.VoiceCommandRequest.CommandId);

        var entity = await _db.CommandRequestItem.FindAsync(request.VoiceCommandRequest.CommandId);

        if (entity == null)
        {
            _logger.LogError("Voice Command was not found: {VoiceCommandRequest}", request);
            return;
        }

        try
        {
            // Начало обработки
            entity.Status = CommandRequestStatus.Processing;
            await _db.SaveChangesAsync();

            var result = await _speechClient.SendCommand(request.VoiceCommandRequest);

            if (result == null)
            {
                _logger.LogError("Voice Command was not processed: {VoiceCommandRequest}", request);
                entity.Status = CommandRequestStatus.Failed;
                return;
            }

            // Команда обработана
            entity.Intent = result.CommandIntent;
            entity.Payload = JsonSerializer.Serialize(result.Entities);
            entity.Status = CommandRequestStatus.Accepted;
            _logger.LogInformation("Voice command successfully processed: {CommandRequestId}", request.VoiceCommandRequest.CommandId);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while processing voice command: {VoiceCommandRequest}", request);
            entity.Status = CommandRequestStatus.Failed;
        }
        finally
        {
            await _db.SaveChangesAsync();
        }
    }

    public static GetVoiceTaskStatusResponse BuildResponse(
        CommandIntent intent,
        object? payloadData = null)
    {
        IVoiceTaskPayload? payload = intent switch
        {
            CommandIntent.TaskCreate => payloadData as TaskCreateData,
            CommandIntent.TaskUpdate => payloadData as TaskUpdateData,
            CommandIntent.TaskDelete => payloadData as TaskDeleteData,
            CommandIntent.TaskQuery => payloadData as TaskQueryData,
            _ => null
        };

        return new GetVoiceTaskStatusResponse(intent, payload);
    }
}
