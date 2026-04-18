using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;
using TaskManager.IntegrationTests.FakeServices;
using TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Interfaces;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

namespace TaskManager.IntegrationTests.Factories;

public class TestFixture
{
    public IServiceProvider ServiceProvider { get; }

    public TestFixture()
    {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(CreateTaskCommand).Assembly, 
                    typeof(CreateRuleCommand).Assembly,
                    typeof(CreateNotificationCommand).Assembly,
                    typeof(CreateCalendarEventCommand).Assembly
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

        ServiceProvider = services.BuildServiceProvider();
    }
}
