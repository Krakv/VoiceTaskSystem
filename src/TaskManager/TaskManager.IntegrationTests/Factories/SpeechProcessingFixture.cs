using CliWrap;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.RegisterUser;
using TaskManager.Auth.Application.Interfaces;
using TaskManager.Auth.Application.Services;
using TaskManager.Auth.Config;
using TaskManager.Auth.Infrastructure;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.Calendar.Infrastructure.Interfaces;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using TaskManager.TaskManagement.Application.Services;
using TaskManager.TaskManagement.Application.Services.Interfaces;
using TaskManager.TaskManagement.Application.Services.VoiceProcessing;
using TaskManager.TaskManagement.Config;
using Testcontainers.PostgreSql;

namespace TaskManager.IntegrationTests.Factories;

public class SpeechProcessingFixture : IAsyncLifetime
{
    public string SpeechServiceBaseUrl { get; } = "http://localhost:3001";
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:17").Build();
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    private static string GetComposeDirectory()
    {
        var baseDir = AppContext.BaseDirectory;

        var path = Path.GetFullPath(Path.Combine(
            baseDir,
            "..", "..", "..", "..", "..", "..",
            "docker"
        ));

        return path;
    }

    public async Task InitializeAsync()
    {
        var composeDir = GetComposeDirectory();

        await Cli.Wrap("docker")
            .WithArguments([
                "compose",
                "-f", "docker-compose.test.yml",
                "-p", "integration-tests",
                "up", "-d", "--build", "--wait"
            ])
            .WithWorkingDirectory(composeDir)
            .WithValidation(CommandResultValidation.ZeroExitCode)
            .ExecuteAsync();

        await _db.StartAsync();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(_db.GetConnectionString()));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(CreateTaskCommand).Assembly,
                typeof(CreateRuleCommand).Assembly,
                typeof(CreateNotificationCommand).Assembly,
                typeof(CreateCalendarEventCommand).Assembly,
                typeof(RegisterUserCommand).Assembly
                );
        });

        var descriptors = services
            .Where(d => d.ServiceType.IsGenericType &&
                        d.ServiceType.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
            .ToList();

        foreach (var d in descriptors)
        {
            services.Remove(d);
        }

        services.AddSingleton<IRuleValidator, FakeRuleValidator>();

        services.AddLogging();

        services.AddScoped<ICurrentUser>(_ => new FakeCurrentUser());

        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddSingleton<IEmailService, FakeEmailService>();
        services.Configure<FrontendOptions>(opt =>
        {
            opt.Url = "http://localhost:3000";
        });
        services.Configure<JwtSettings>(opt =>
        {
            opt.Secret = "THIS_IS_TEST_SECRET_KEY_123456789";
            opt.Issuer = "test-issuer";
            opt.Audience = "test-audience";
            opt.ExpiryMinutes = 60;
        });
        services.Configure<SpeechProcessingConfig>(opt =>
        {
            opt.APIURI = SpeechServiceBaseUrl;
        });

        services.AddSingleton<ICalDavClient, FakeCalDavClient>();
        services.AddSingleton<IStateService, OAuthStateService>();

        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<VoiceProcessingWorker>();
        services.AddScoped<VoiceProcessingHandler>();

        services.AddHttpClient<YandexOAuthClient>().ConfigurePrimaryHttpMessageHandler<FakeYandexOAuthHandler>();
        services.AddSingleton<FakeYandexOAuthHandler>();

        services.Configure<YandexOAuthConfig>(opt =>
        {
            opt.ClientId = "test_client_id";
            opt.ClientSecret = "test_secret";
            opt.TokenUrl = "https://oauth.yandex.ru/token";
            opt.AuthorizeUri = "https://oauth.yandex.ru/authorize";
            opt.RedirectUri = "http://localhost/callback";
        });

        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await ctx.Database.MigrateAsync();

        ServiceProvider = provider;
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();

        var composeDir = GetComposeDirectory();

        await Cli.Wrap("docker")
            .WithArguments([
                "compose",
                "-f", "docker-compose.test.yml",
                "-p", "integration-tests",
                "down", "-v"
            ])
            .WithWorkingDirectory(composeDir)
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();
    }

    public async Task<Guid> CreateUserAsync(string name = "test")
    {
        using var scope = ServiceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user = new User { Id = Guid.NewGuid(), UserName = $"{name}_{Guid.NewGuid()}@test.com", Email = $"{Guid.NewGuid()}@mail.com" };
        await userManager.CreateAsync(user, "Password123!");

        return user.Id;
    }
}
