using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using WebSocketMockServer.Scheduling;

using Xunit;

namespace WebSocketMockServer.Tests;

public class WorkSchedulerTests
{
    [Fact(DisplayName = $"Unable to create {nameof(WorkScheduler)} with null logger.")]
    [Trait("Category", "Unit")]
    public void CantCreateWorkSchedulerWithNullLogger()
    {
        // Act
        var exception = Record.Exception(
            () => new WorkScheduler(null!));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = $"{nameof(WorkScheduler)} could be created.")]
    [Trait("Category", "Unit")]
    public void WorkSchedulerCanBeCreated()
    {
        // Act
        var exception = Record.Exception(
            () => new WorkScheduler(Mock.Of<ILogger<WorkScheduler>>()));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = $"{nameof(WorkScheduler)} can't schedule null work.")]
    [Trait("Category", "Unit")]
    public void WorkSchedulerCantScheduleNull()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var scheduler = new WorkScheduler(Mock.Of<ILogger<WorkScheduler>>());

        // Act
        var exception = Record.Exception(
            () => scheduler.Schedule(null!, cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = $"{nameof(WorkScheduler)} can't schedule if cancel.")]
    [Trait("Category", "Unit")]
    public void WorkSchedulerCantScheduleIfCancel()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var scheduler = new WorkScheduler(Mock.Of<ILogger<WorkScheduler>>());

        // Act
        var exception = Record.Exception(
            () => scheduler.Schedule(() => Task.CompletedTask, cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<OperationCanceledException>();
    }

    [Fact(DisplayName = $"{nameof(WorkScheduler)} can schedule work.")]
    [Trait("Category", "Unit")]
    public async Task WorkSchedulerCanScheduleWorkAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var tcs = new TaskCompletionSource();

        var scheduler = new WorkScheduler(Mock.Of<ILogger<WorkScheduler>>());

        // Act
        scheduler.Schedule(() =>
        {
            tcs.SetResult();
            return Task.CompletedTask;
        }, cts.Token);

        // Assert
        await tcs.Task;
    }

    [Fact(DisplayName = $"{nameof(WorkScheduler)} can schedule work with error.")]
    [Trait("Category", "Unit")]
    public async Task WorkSchedulerCanScheduleWorkWithErrorAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var tcs = new TaskCompletionSource();

        var scheduler = new WorkScheduler(Mock.Of<ILogger<WorkScheduler>>());

        // Act
        scheduler.Schedule(() =>
        {
            tcs.SetResult();
            throw new InvalidOperationException();
        }, cts.Token);

        // Assert
        await tcs.Task;
    }

}
