using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DeleteVoiceTask;

namespace TaskManager.IntegrationTests.TaskManagement.CommandRequestFeature;

public class DeleteVoiceTaskHandlerTests(TestFixture fixture)
    : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture = fixture;

    [Fact]
    public async Task Handle_CompletedCommand_CancelsSuccessfully()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("delete_success");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            CreatedAt = DateTime.UtcNow
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var response = await mediator.Send(
            new DeleteVoiceTaskCommand(ownerId, command.CommandRequestId));

        var updated = await db.CommandRequestItem.FindAsync(command.CommandRequestId);

        Assert.NotNull(updated);
        Assert.Equal(CommandRequestStatus.Cancelled, updated.Status);
        Assert.NotNull(updated.CancelledAt);
        Assert.Equal(command.CommandRequestId, response.CommandRequestId);
    }

    [Fact]
    public async Task Handle_CommandNotFound_ThrowsValidationException()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var ownerId = await _fixture.CreateUserAsync("delete_not_found");

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new DeleteVoiceTaskCommand(ownerId, Guid.NewGuid())));

        Assert.Equal("NOT_FOUND", ex.ErrorCode);
    }

    [Fact]
    public async Task Handle_AlreadyCancelled_ThrowsValidationException()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("delete_cancelled");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.Cancelled
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new DeleteVoiceTaskCommand(ownerId, command.CommandRequestId)));

        Assert.Equal("CANCELLED", ex.ErrorCode);
    }

    [Fact]
    public async Task Handle_PendingCommand_ThrowsValidationException()
    {
        await AssertStatusThrows(CommandRequestStatus.Pending, "PENDING");
    }

    [Fact]
    public async Task Handle_ProcessingCommand_ThrowsValidationException()
    {
        await AssertStatusThrows(CommandRequestStatus.Processing, "PENDING");
    }

    [Fact]
    public async Task Handle_AcceptedCommand_ThrowsValidationException()
    {
        await AssertStatusThrows(CommandRequestStatus.Accepted, "ALREADY_CONFIRMED");
    }

    [Fact]
    public async Task Handle_FailedCommand_ThrowsValidationException()
    {
        await AssertStatusThrows(CommandRequestStatus.Failed, "INTERNAL_SERVER_ERROR");
    }

    private async Task AssertStatusThrows(CommandRequestStatus status, string expectedCode)
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync($"status_{status}");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = status
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new DeleteVoiceTaskCommand(ownerId, command.CommandRequestId)));

        Assert.Equal(expectedCode, ex.ErrorCode);
    }
}
