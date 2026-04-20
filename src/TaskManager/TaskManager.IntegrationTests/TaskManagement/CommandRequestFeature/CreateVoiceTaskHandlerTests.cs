using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using TaskManager.IntegrationTests.Factories;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Requests;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.CreateVoiceTask;
using TaskManager.TaskManagement.Application.Services.Interfaces;

namespace TaskManager.IntegrationTests.TaskManagement.CommandRequestFeature;

public class CreateVoiceTaskHandlerTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture = fixture;

    [Fact]
    public async Task Handle_ValidRequest_SavesCommandRequestToDatabase()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var queue = scope.ServiceProvider.GetRequiredService<IBackgroundTaskQueue>();

        var ownerId = await _fixture.CreateUserAsync("voice_task_user");

        var content = new byte[] { 0x01, 0x02, 0x03 };
        var inputFile = new InputFile(
            "audio.ogg",
            "audio/ogg",
            content.Length,
            content
        );

        var response = await mediator.Send(new CreateVoiceTaskCommand(ownerId, inputFile));

        var saved = await dbContext.CommandRequestItem.FindAsync(response.CommandRequestId);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var _ = await queue.DequeueAsync(cts.Token);

        Assert.NotNull(saved);
        Assert.Equal(ownerId, saved.OwnerId);
        Assert.Equal(CommandRequestStatus.Pending, saved.Status);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsCorrectCommandRequestId()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var queue = scope.ServiceProvider.GetRequiredService<IBackgroundTaskQueue>();

        var ownerId = await _fixture.CreateUserAsync("voice_task_user_2");
        var content = new byte[] { 0x01, 0x02, 0x03 };
        var inputFile = new InputFile(
            "audio.ogg",
            "audio/ogg",
            content.Length,
            content
        );

        var response = await mediator.Send(new CreateVoiceTaskCommand(ownerId, inputFile));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var _ = await queue.DequeueAsync(cts.Token);

        Assert.NotEqual(Guid.Empty, response.CommandRequestId);
        Assert.Equal("Voice task creation requested successfully.", response.Message);
    }

    [Fact]
    public async Task Handle_ValidRequest_EnqueuesVoiceCommandDto()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var queue = scope.ServiceProvider.GetRequiredService<IBackgroundTaskQueue>();

        var ownerId = await _fixture.CreateUserAsync("voice_task_user_3");
        var content = new byte[] { 0x01, 0x02, 0x03 };
        var inputFile = new InputFile(
            "audio.ogg",
            "audio/ogg",
            content.Length,
            content
        );

        var response = await mediator.Send(new CreateVoiceTaskCommand(ownerId, inputFile));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var job = await queue.DequeueAsync(cts.Token);

        Assert.Equal(ownerId, job.UserId);
        Assert.Equal(response.CommandRequestId, job.VoiceCommandRequest.CommandId);
    }

    [Fact]
    public async Task Handle_ValidRequest_CommandRequestHasCorrectDefaultValues()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var queue = scope.ServiceProvider.GetRequiredService<IBackgroundTaskQueue>();

        var ownerId = await _fixture.CreateUserAsync("voice_task_defaults");
        var content = new byte[] { 0xFF };
        var inputFile = new InputFile(
            "audio.ogg",
            "audio/ogg",
            content.Length,
            content
        );

        var before = DateTimeOffset.UtcNow;

        var response = await mediator.Send(new CreateVoiceTaskCommand(ownerId, inputFile));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var _ = await queue.DequeueAsync(cts.Token);

        var after = DateTimeOffset.UtcNow;

        var saved = await dbContext.CommandRequestItem.FindAsync(response.CommandRequestId);

        Assert.NotNull(saved);
        Assert.Null(saved.Intent);
        Assert.Null(saved.Payload);
        Assert.False(saved.ConfirmationRequired);
        Assert.Null(saved.UpdatedAt);
        Assert.Null(saved.CancelledAt);
        Assert.Null(saved.AcceptedAt);
        Assert.InRange(saved.CreatedAt, before, after);
    }
}
