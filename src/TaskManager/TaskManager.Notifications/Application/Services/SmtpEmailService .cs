using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Notifications.Config;

namespace TaskManager.Notifications.Application.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<SmtpOptions> options,
        ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("Recipient email is empty", nameof(to));

        if (string.IsNullOrWhiteSpace(_options.From))
            throw new ArgumentException("SMTP From is empty");

        if (string.IsNullOrWhiteSpace(_options.Username))
            throw new ArgumentException("SMTP Username is empty");

        if (string.IsNullOrWhiteSpace(_options.Password))
            throw new ArgumentException("SMTP Password is empty");

        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Task Manager", _options.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new SmtpClient();

        _logger.LogDebug("Connecting to SMTP {Host}:{Port}", _options.Host, _options.Port);

        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            SecureSocketOptions.SslOnConnect);

        _logger.LogDebug("Authenticating SMTP user {User}", _options.Username);

        await client.AuthenticateAsync(_options.Username, _options.Password);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);

        _logger.LogDebug("Email sent -> To={To}", to);
    }
}