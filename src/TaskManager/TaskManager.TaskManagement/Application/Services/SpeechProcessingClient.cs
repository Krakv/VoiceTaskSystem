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

    public async Task<CommandResponse?> SendCommand(VoiceCommandRequest command)
    {
        using var content = new MultipartFormDataContent();

        var streamContent = new StreamContent(new MemoryStream(command.FormFile.Content));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(command.FormFile.ContentType);

        content.Add(streamContent, "FormFile", command.FormFile.FileName);
        content.Add(new StringContent(command.CommandId.ToString()), "CommandRequestId");
        content.Add(new StringContent(command.UserTimeZone), "UserTimeZone");

        var response = await _httpClient.PostAsync(_conf.Value.APIURI, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CommandResponse>();
    }
}
