using Microsoft.Extensions.Options;
using TaskManager.Application.Common.DTOs.Requests;
using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.TaskManagement.Application.Services.Interfaces;
using TaskManager.TaskManagement.Config;
using Telegram.Bot.Requests.Abstractions;

namespace TaskManager.TaskManagement.Application.Services;

public class SpeechProcessingClient(HttpClient httpClient, IOptions<SpeechProcessingConfig> conf) : ISpeechProcessingClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IOptions<SpeechProcessingConfig> _conf = conf;

    public async Task<CommandResponse?> SendCommand(CommandRequest command)
    {
        var response = await _httpClient.PostAsJsonAsync(_conf.Value.APIURI, command);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CommandResponse>();
    }
}
