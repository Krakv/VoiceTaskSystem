using GigaChatAdapter;
using Microsoft.Extensions.Options;
using SpeechProcessingService.Application.Services.Interfaces;
using SpeechProcessingService.Config;

namespace SpeechProcessingService.Application.Services;

public class GigaChatService : IGenAIService
{
    private readonly Authorization _auth;

    public GigaChatService(IOptions<GigaChatCredentials> credentials)
    {
        string authData = credentials.Value.AuthToken; // base64
        _auth = new Authorization(authData, GigaChatAdapter.Auth.RateScope.GIGACHAT_API_PERS);
        Authorize().GetAwaiter().GetResult();
    }

    private async Task Authorize()
    {
        Console.WriteLine("Started Auth ");
        var authResult = await _auth.SendRequest();

        if (!authResult.AuthorizationSuccess)
        {
            throw new ArgumentException(authResult.ErrorTextIfFailed);
        }
    }

    public async Task<string> GetAnswer(string prompt)
    {
        Completion completion = new Completion();

        //update access token if expired / Обновление токена, если он просрочился
        await _auth.UpdateToken();

        //request / отправка промпта. Чтобы исключить историю переписки - необходимо в методе указать false для аргумента UseHistory (по умолчанию UseHistory = true)
        var result = await completion.SendRequest(_auth.LastResponse.GigaChatAuthorizationResponse?.AccessToken, prompt, useHistory: false);

        if (result.RequestSuccessed)
        {
            Console.WriteLine(result.GigaChatCompletionResponse.Choices.LastOrDefault().Message.Content);
            return result.GigaChatCompletionResponse.Choices.LastOrDefault().Message.Content;
        }

        Console.WriteLine(result.ErrorTextIfFailed);
        return result.ErrorTextIfFailed;
    }
}
