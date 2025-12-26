using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.DTOs.Requests;
using TaskManager.Application.Common.DTOs.Responses;

namespace TaskManager.Application.Services.Interfaces;

public interface IIntentDispatcher
{
    Task<IntentResult> DispatchAsync(IntentCommand command);
}
