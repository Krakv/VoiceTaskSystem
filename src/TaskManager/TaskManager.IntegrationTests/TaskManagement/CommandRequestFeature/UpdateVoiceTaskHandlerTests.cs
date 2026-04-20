using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;

namespace TaskManager.IntegrationTests.TaskManagement.CommandRequestFeature;

public class UpdateVoiceTaskHandlerTests(TestFixture fixture)
    : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture = fixture;

    [Fact]
    public async Task Handle_TaskCreate_UpdatesFieldsAndReturnsUpdatedFields()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("update_create");

        var payload = new TaskCreateData(
            ProjectName: null,
            Title: "old",
            Description: "old",
            Status: TaskItemStatus.New,
            Priority: TaskItemPriority.Medium,
            DueDate: null,
            Message: "test",
            ParentTaskId: null,
            ParentTask: null
        );

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

        var response = await mediator.Send(new UpdateVoiceTaskCommand(
            ownerId,
            command.CommandRequestId,
            null,
            null,
            "new title",
            "new description",
            null,
            null,
            null,
            null));

        var updated = await db.CommandRequestItem.FindAsync(command.CommandRequestId);
        var updatedPayload = JsonSerializer.Deserialize<TaskCreateData>(updated!.Payload!);

        Assert.Equal("new title", updatedPayload!.Title);
        Assert.Equal("new description", updatedPayload.Description);
        Assert.NotNull(updated.UpdatedAt);
        Assert.Contains("Title", response.UpdatedFields.Keys);
    }

    [Fact]
    public async Task Handle_TaskUpdate_UpdatesTasksFromDatabase()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("update_task");

        var task = new TaskItem
        {
            TaskId = Guid.NewGuid(),
            OwnerId = ownerId,
            Title = "db task",
            Status = TaskItemStatus.New,
            Priority = TaskItemPriority.Medium,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.TaskItems.Add(task);

        var payload = new TaskUpdateData(
            Tasks: new List<TaskShortInfoDto>(),
            ProjectName: null,
            Description: null,
            Status: null,
            Priority: null,
            DueDate: null,
            ParentTaskId: null,
            ParentTask: null
        );

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            Intent = CommandIntent.TaskUpdate,
            Payload = JsonSerializer.Serialize(payload)
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var response = await mediator.Send(new UpdateVoiceTaskCommand(
            ownerId,
            command.CommandRequestId,
            task.TaskId,
            null, null, null, null, null, null, null));

        var updated = await db.CommandRequestItem.FindAsync(command.CommandRequestId);
        var updatedPayload = JsonSerializer.Deserialize<TaskUpdateData>(updated!.Payload!);

        Assert.Single(updatedPayload!.Tasks);
        Assert.Equal(task.TaskId, updatedPayload.Tasks[0].TaskId);
        Assert.Contains("Tasks", response.UpdatedFields.Keys);
    }

    [Fact]
    public async Task Handle_TaskDelete_UpdatesTasks()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("update_delete");

        var payload = new TaskDeleteData(
            Tasks: new List<TaskShortInfoDto>()
        );

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            Intent = CommandIntent.TaskDelete,
            Payload = JsonSerializer.Serialize(payload)
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var response = await mediator.Send(new UpdateVoiceTaskCommand(
            ownerId,
            command.CommandRequestId,
            null, null, null, null, null, null, null, null));

        Assert.NotNull(response);
    }

    [Fact]
    public async Task Handle_InvalidIntent_ThrowsValidationException()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ownerId = await _fixture.CreateUserAsync("invalid_intent");

        var command = new CommandRequestItem
        {
            CommandRequestId = Guid.NewGuid(),
            OwnerId = ownerId,
            Status = CommandRequestStatus.WaitingForConfirmation,
            Intent = CommandIntent.Unknown,
            Payload = "{}"
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new UpdateVoiceTaskCommand(
                ownerId,
                command.CommandRequestId,
                null, null, null, null, null, null, null, null)));
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsValidationException()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var ownerId = await _fixture.CreateUserAsync("not_found");

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new UpdateVoiceTaskCommand(
                ownerId,
                Guid.NewGuid(),
                null, null, null, null, null, null, null, null)));

        Assert.Equal("NOT_FOUND", ex.ErrorCode);
    }

    [Fact]
    public async Task Handle_Pending_ThrowsValidationException()
    {
        await AssertStatusThrows(CommandRequestStatus.Pending, "PENDING");
    }

    [Fact]
    public async Task Handle_Cancelled_ThrowsValidationException()
    {
        await AssertStatusThrows(CommandRequestStatus.Cancelled, "CANCELLED");
    }

    [Fact]
    public async Task Handle_Accepted_ThrowsValidationException()
    {
        await AssertStatusThrows(CommandRequestStatus.Accepted, "ALREADY_CONFIRMED");
    }

    [Fact]
    public async Task Handle_Failed_ThrowsValidationException()
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
            Status = status,
            Intent = CommandIntent.TaskCreate,
            Payload = "{}"
        };

        db.CommandRequestItem.Add(command);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ValidationAppException>(() =>
            mediator.Send(new UpdateVoiceTaskCommand(
                ownerId,
                command.CommandRequestId,
                null, null, null, null, null, null, null, null)));

        Assert.Equal(expectedCode, ex.ErrorCode);
    }
}
