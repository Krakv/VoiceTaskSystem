namespace TaskManager.Auth.Config;

public class YandexOAuthConfig
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string AuthorizeUri { get; set; } = string.Empty;
}
