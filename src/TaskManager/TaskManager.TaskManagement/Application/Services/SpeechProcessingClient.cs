using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManager.Shared.DTOs.Requests;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.TaskManagement.Application.Services.Interfaces;
using TaskManager.TaskManagement.Config;

namespace TaskManager.TaskManagement.Application.Services;

public class SpeechProcessingClient(HttpClient httpClient, IOptions<SpeechProcessingConfig> conf, ILogger<SpeechProcessingClient> logger) : ISpeechProcessingClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IOptions<SpeechProcessingConfig> _conf = conf;
    private readonly ILogger<SpeechProcessingClient> _logger = logger;

    public async Task<CommandResponse?> SendCommand(VoiceCommandRequest command)
    {
        _logger.LogInformation("Started");
        using var content = new MultipartFormDataContent();

        var streamContent = new StreamContent(new MemoryStream(command.FormFile.Content));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(command.FormFile.ContentType);

        content.Add(streamContent, "FormFile", command.FormFile.FileName);
        content.Add(new StringContent(command.CommandId.ToString()), "CommandRequestId");
        content.Add(new StringContent(command.UserTimeZone), "UserTimeZone");

        var response = await _httpClient.PostAsync(_conf.Value.APIURI, content);
        response.EnsureSuccessStatusCode(); 
        _logger.LogInformation("Get response: {CommandResponse}", await response.Content.ReadAsStringAsync());
        var commandResponse = await response.Content.ReadFromJsonAsync<CommandResponse>(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );
        return commandResponse;
    }
}
