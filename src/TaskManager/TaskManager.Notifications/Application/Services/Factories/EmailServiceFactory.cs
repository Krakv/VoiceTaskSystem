using TaskManager.Notifications.Application.Services.Interfaces;

namespace TaskManager.Notifications.Application.Services.Factories;

public abstract class EmailServiceFactory
{
    public abstract IEmailService CreateEmailService();
}
