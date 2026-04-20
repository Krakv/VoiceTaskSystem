using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.ConfirmVoiceTask;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;
using TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;
using TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

namespace TaskManager.IntegrationTests.TaskManagement.CommandRequestFeature;

public class ConfirmVoiceTaskHandlerTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Handle_TaskCreate_CallsMediator_AndSetsAccepted()
    {
        var db = CreateDbContext();

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTaskCommand>(), default))
            .ReturnsAsync(Guid.NewGuid());

        var logger = Mock.Of<ILogger<ConfirmVoiceTaskHandler>>();

        var handler = new ConfirmVoiceTaskHandler(db, logger, mediatorMock.Object);

        var ownerId = Guid.NewGuid();

        var payload = new TaskCreateData(
            null,
            "title",
            null,
            TaskItemStatus.New,
            TaskItemPriority.Medium,
            null,
            "msg",
            null,
            null
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

        var result = await handler.Handle(
            new ConfirmVoiceTaskCommand(ownerId, command.CommandRequestId),
            CancellationToken.None);

        var updated = await db.CommandRequestItem.FirstAsync();

        Assert.Equal(CommandRequestStatus.Accepted, updated.Status);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateTaskCommand>(), default), Moq.Times.Once);
    }

    [Fact]
    public async Task Handle_TaskUpdate_CallsMediator()
    {
        var db = CreateDbContext();

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateTaskPatchCommand>(), default))
            .Returns(Task.FromResult(Guid.NewGuid()));

        var handler = new ConfirmVoiceTaskHandler(
            db,
            Mock.Of<ILogger<ConfirmVoiceTaskHandler>>(),
            mediatorMock.Object);

        var ownerId = Guid.NewGuid();

        var payload = new TaskUpdateData(
            new List<TaskShortInfoDto>
            {
                new TaskShortInfoDto(Guid.NewGuid(), "t", TaskItemStatus.New, TaskItemPriority.Medium, null)
            },
            null, null, null, null, null, null, null
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

        await handler.Handle(new ConfirmVoiceTaskCommand(ownerId, command.CommandRequestId), CancellationToken.None);

        mediatorMock.Verify(m => m.Send(It.IsAny<UpdateTaskPatchCommand>(), default), Moq.Times.Once);
    }

    [Fact]
    public async Task Handle_TaskDelete_CallsMediator()
    {
        var db = CreateDbContext();

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteTaskCommand>(), default))
            .Returns(Task.FromResult(Guid.NewGuid()));

        var handler = new ConfirmVoiceTaskHandler(
            db,
            Mock.Of<ILogger<ConfirmVoiceTaskHandler>>(),
            mediatorMock.Object);

        var ownerId = Guid.NewGuid();

        var payload = new TaskDeleteData(
            new List<TaskShortInfoDto>
            {
                new TaskShortInfoDto(Guid.NewGuid(), "t", TaskItemStatus.New, TaskItemPriority.Medium, null)
            }
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

        await handler.Handle(new ConfirmVoiceTaskCommand(ownerId, command.CommandRequestId), CancellationToken.None);

        mediatorMock.Verify(m => m.Send(It.IsAny<DeleteTaskCommand>(), default), Moq.Times.Once);
    }

    [Fact]
    public async Task Handle_NotFound_Throws()
    {
        var db = CreateDbContext();

        var handler = new ConfirmVoiceTaskHandler(
            db,
            Mock.Of<ILogger<ConfirmVoiceTaskHandler>>(),
            Mock.Of<IMediator>());

        await Assert.ThrowsAsync<ValidationAppException>(() =>
            handler.Handle(new ConfirmVoiceTaskCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Exception_SetsFailedStatus()
    {
        var db = CreateDbContext();

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTaskCommand>(), default))
            .ThrowsAsync(new Exception("boom"));

        var handler = new ConfirmVoiceTaskHandler(
            db,
            Mock.Of<ILogger<ConfirmVoiceTaskHandler>>(),
            mediatorMock.Object);

        var ownerId = Guid.NewGuid();

        var payload = new TaskCreateData(
            null,
            "title",
            null,
            TaskItemStatus.New,
            TaskItemPriority.Medium,
            null,
            "msg",
            null,
            null
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

        await Assert.ThrowsAsync<Exception>(() =>
            handler.Handle(new ConfirmVoiceTaskCommand(ownerId, command.CommandRequestId), CancellationToken.None));

        var updated = await db.CommandRequestItem.FirstAsync();

        Assert.Equal(CommandRequestStatus.Failed, updated.Status);
    }
}
