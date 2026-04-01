using Microsoft.Extensions.Options;
using TaskManager.Shared.DTOs.Requests;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.TaskManagement.Application.Services.Interfaces;
using TaskManager.TaskManagement.Config;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace TaskManager.TaskManagement.Application.Services;

public class SpeechProcessingClient(HttpClient httpClient, IOptions<SpeechProcessingConfig> conf) : ISpeechProcessingClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IOptions<SpeechProcessingConfig> _conf = conf;

    public async Task<CommandResponse?> SendCommand(CommandRequest command)
    {
        using var content = new MultipartFormDataContent();

        var streamContent = new StreamContent(command.InputFile.Content);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(command.InputFile.ContentType);

        content.Add(streamContent, "file", command.InputFile.FileName);

        var response = await _httpClient.PostAsJsonAsync(_conf.Value.APIURI, command);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CommandResponse>();
    }
}
