using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

namespace TaskManager.IntegrationTests.TaskManagement.CommandRequestFeature;

public class GetVoiceTaskStatusHandlerTests(TestFixture fixture)
    : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture = fixture;

    [Fact]
    public async Task Handle_ValidTaskCreate_ReturnsPayload()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("status_ok");

        var payload = new TaskCreateData(ProjectName: null, Title: "Test task", Description: null, Status: TaskItemStatus.New, Priority: TaskItemPriority.Low, DueDate: null, Message: "???????", ParentTask: null, ParentTaskId: null);

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            Intent = CommandIntent.TaskCreate,
            Payload = JsonSerializer.Serialize(payload)
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var response = await mediator.Send(
            new GetVoiceTaskStatusQuery(ownerId, command.CommandRequestId));

        Assert.Equal(CommandIntent.TaskCreate, response.Intent);
        Assert.NotNull(response.Payload);

        var result = Assert.IsType<TaskCreateData>(response.Payload);
        Assert.Equal("Test task", result.Title);
    }

    [Fact]
    public async Task Handle_CommandNotFound_Throws()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var ownerId = await _fixture.CreateUserAsync("status_not_found");

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new GetVoiceTaskStatusQuery(ownerId, Guid.NewGuid())));

        Assert.Equal("NOT_FOUND", ex.ErrorCode);
    }

    [Fact]
    public async Task Handle_Pending_Throws()
    {
        await AssertStatusThrows(CommandRequestStatus.Pending, "PENDING");
    }

    [Fact]
    public async Task Handle_Processing_Throws()
    {
        await AssertStatusThrows(CommandRequestStatus.Processing, "PENDING");
    }

    [Fact]
    public async Task Handle_Failed_Throws()
    {
        await AssertStatusThrows(CommandRequestStatus.Failed, "INTERNAL_SERVER_ERROR");
    }

    [Fact]
    public async Task Handle_NullIntent_Throws()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("null_intent");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            Intent = null,
            Payload = "{}"
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new GetVoiceTaskStatusQuery(ownerId, command.CommandRequestId)));

        Assert.Equal("INTERNAL_SERVER_ERROR", ex.ErrorCode);
    }

    [Fact]
    public async Task Handle_NullPayload_Throws()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("null_payload");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            Intent = CommandIntent.TaskCreate,
            Payload = null
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new GetVoiceTaskStatusQuery(ownerId, command.CommandRequestId)));

        Assert.Equal("INTERNAL_SERVER_ERROR", ex.ErrorCode);
    }

    [Fact]
    public async Task Handle_UnknownIntent_ReturnsMessageData()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("unknown_intent");

        var payload = new MessageData("??? ");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            Intent = CommandIntent.Unknown,
            Payload = JsonSerializer.Serialize(payload)
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var response = await mediator.Send(
            new GetVoiceTaskStatusQuery(ownerId, command.CommandRequestId));

        Assert.NotNull(response.Payload);
        Assert.IsType<MessageData>(response.Payload);
    }

    private async Task AssertStatusThrows(CommandRequestStatus status, string code)
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync($"status_{status}");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = status,
            Intent = CommandIntent.TaskCreate,
            Payload = "{}"
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new GetVoiceTaskStatusQuery(ownerId, command.CommandRequestId)));

        Assert.Equal(code, ex.ErrorCode);
    }
}
