using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using WebSocketMockServer.Scheduling;

using Xunit;

namespace WebSocketMockServer.Tests;

public class WorkShedulerTests
{
    [Fact(DisplayName = "Unable to create WorkSheduler with null logger.")]
    [Trait("Category", "Unit")]
    public void CantCreateWorkShedulerWithNullLogger()
    {
        // Act
        var exception = Record.Exception(
            () => new WorkSheduler(null!));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "WorkSheduler could be created.")]
    [Trait("Category", "Unit")]
    public void WorkShedulerCanBeCreated()
    {
        // Act
        var exception = Record.Exception(
            () => new WorkSheduler(Mock.Of<ILogger<WorkSheduler>>()));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "WorkSheduler can't shedule null work.")]
    [Trait("Category", "Unit")]
    public void WorkShedulerCantSheduleNull()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var scheduler = new WorkSheduler(Mock.Of<ILogger<WorkSheduler>>());

        // Act
        var exception = Record.Exception(
            () => scheduler.Schedule(null!, cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "WorkSheduler can't shedule if cancel.")]
    [Trait("Category", "Unit")]
    public void WorkShedulerCantSheduleIfCancel()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var scheduler = new WorkSheduler(Mock.Of<ILogger<WorkSheduler>>());

        // Act
        var exception = Record.Exception(
            () => scheduler.Schedule(() => Task.CompletedTask, cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<OperationCanceledException>();
    }

    [Fact(DisplayName = "WorkSheduler can shedule work.")]
    [Trait("Category", "Unit")]
    public async Task WorkShedulerCanSheduleWorkAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var tcs = new TaskCompletionSource();

        var scheduler = new WorkSheduler(Mock.Of<ILogger<WorkSheduler>>());

        // Act
        scheduler.Schedule(() =>
        {
            tcs.SetResult();
            return Task.CompletedTask;
        }, cts.Token);

        // Assert
        await tcs.Task;
    }

    [Fact(DisplayName = "WorkSheduler can shedule work with error.")]
    [Trait("Category", "Unit")]
    public async Task WorkShedulerCanSheduleWorkWithErrorAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var tcs = new TaskCompletionSource();

        var scheduler = new WorkSheduler(Mock.Of<ILogger<WorkSheduler>>());

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
