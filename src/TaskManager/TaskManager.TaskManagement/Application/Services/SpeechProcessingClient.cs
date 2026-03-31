using Microsoft.Extensions.Options;
using TaskManager.Shared.DTOs.Requests;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.TaskManagement.Application.Services.Interfaces;
using TaskManager.TaskManagement.Config;
using System.Net.Http.Json;

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
