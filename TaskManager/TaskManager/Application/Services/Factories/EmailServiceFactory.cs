using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services.Factories;

public abstract class EmailServiceFactory
{
    public abstract IEmailService CreateEmailService();
}
