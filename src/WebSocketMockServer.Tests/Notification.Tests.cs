using FluentAssertions;

using Xunit;

using Moq;

using WebSocketMockServer.WebSockets;
using WebSocketMockServer.Reactions;
using Microsoft.Extensions.Logging;
using WebSocketMockServer.Scheduling;

namespace WebSocketMockServer.Tests;

public class NotificationTests
{
    [Fact(DisplayName = "Notification can not be created with null message.")]
    [Trait("Category", "Unit")]
    public void CantCreateResponseWithNullMessage()
    {
        //Arrange
        string msg = null!;
        var delay = 1;

        // Act
        var exception = Record.Exception(
            () => new Notification(
                                  msg,
                                  TimeSpan.FromMilliseconds(delay),
                                  Mock.Of<IWorkScheduler>(MockBehavior.Strict),
                                  Mock.Of<ILogger<Reaction>>()));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Theory(DisplayName = "Notification can not be created with empty message.")]
    [InlineData("")]
    [InlineData("     ")]
    [Trait("Category", "Unit")]
    public void CantCreateResponseWithEmptyMessage(string msg)
    {
        //Arrange
        var delay = 1;

        // Act
        var exception = Record.Exception(
            () => new Notification(
                                    msg,
                                    TimeSpan.FromMilliseconds(delay),
                                    Mock.Of<IWorkScheduler>(MockBehavior.Strict),
                                    Mock.Of<ILogger<Reaction>>()));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
    }

    [Fact(DisplayName = "Notification can not be created with incorrect delay.")]
    [Trait("Category", "Unit")]
    public void CantCreateResponseWithBadDelay()
    {
        //Arrange
        var msg = "aaa";
        var delay = 0;

        // Act
        var exception = Record.Exception(
            () => new Notification(
                                msg,
                                TimeSpan.FromMilliseconds(delay),
                                Mock.Of<IWorkScheduler>(MockBehavior.Strict),
                                Mock.Of<ILogger<Reaction>>()));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
    }

    [Fact(DisplayName = "Notification can be created with valid message.")]
    [Trait("Category", "Unit")]
    public void ResponseCanBeCreated()
    {
        //Arrange
        var msg = "aaa";
        var delay = 1;

        // Act
        var exception = Record.Exception(
            () => new Notification(
                                msg,
                                TimeSpan.FromMilliseconds(delay),
                                Mock.Of<IWorkScheduler>(MockBehavior.Strict),
                                Mock.Of<ILogger<Reaction>>()));

        // Assert
        exception.Should().BeNull();
    }


    [Fact(DisplayName = "Notification can not be sent with empty socket.")]
    [Trait("Category", "Unit")]
    public async Task CantSendWithEmptyProxyAsync()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        var msg = "Test";
        var delay = 1;
        var reaction = new Notification(
                                    msg,
                                    TimeSpan.FromMilliseconds(delay),
                                    Mock.Of<IWorkScheduler>(MockBehavior.Strict),
                                    Mock.Of<ILogger<Reaction>>());
        var proxy = (IWebSocketProxy)null!;

        // Act
        var exception = await Record.ExceptionAsync(
            () => reaction.SendMessageAsync(proxy, cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "Notification could be sent.")]
    [Trait("Category", "Unit")]
    public void CouldSendNotification()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        var msg = "Test";
        var delay = 1000;
        var scheduler = new Mock<IWorkScheduler>();
        var reaction = new Notification(
                                     msg,
                                     TimeSpan.FromMilliseconds(delay),
                                     scheduler.Object,
                                     Mock.Of<ILogger<Reaction>>());

        // Act
        var t = reaction.SendMessageAsync(
                                Mock.Of<IWebSocketProxy>(MockBehavior.Strict),
                                cts.Token);

        // Assert
        t.IsCompleted.Should().BeTrue();
        scheduler.Verify(x => x.Schedule(
                                It.IsAny<Func<Task>>(), cts.Token),
                                Times.Once);
    }

    [Fact(DisplayName = "Notification cant be sent on cancelled token.")]
    [Trait("Category", "Unit")]
    public async Task CantSendNotificationOnCanceledTokenAsync()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var msg = "Test";
        var delay = 1000;
        var scheduler = new Mock<IWorkScheduler>();
        var reaction = new Notification(
                                    msg,
                                    TimeSpan.FromMilliseconds(delay),
                                    scheduler.Object,
                                    Mock.Of<ILogger<Reaction>>());

        // Act
        var exception = await Record.ExceptionAsync(
            () => reaction.SendMessageAsync(
                                    Mock.Of<IWebSocketProxy>(MockBehavior.Strict),
                                    cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<OperationCanceledException>();
    }
}
