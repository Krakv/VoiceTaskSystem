using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Auth.Application.Features.Auth.RegisterUser;
using TaskManager.Auth.Config;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using Testcontainers.PostgreSql;

namespace TaskManager.IntegrationTests.Factories;

public class TestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:17").Build();
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public async Task InitializeAsync()
    {
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

        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await ctx.Database.MigrateAsync();

        ServiceProvider = provider;
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
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
