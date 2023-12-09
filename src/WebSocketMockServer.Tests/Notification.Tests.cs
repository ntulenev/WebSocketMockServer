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
    public void CantCreatResponseWithNullMessage()
    {
        //Arrange
        string msg = null!;
        var delay = 1;

        // Act
        var exception = Record.Exception(
            () => new Notification(msg, delay, Mock.Of<IWorkSheduler>(MockBehavior.Strict), Mock.Of<ILogger<Reaction>>()));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Theory(DisplayName = "Notification can not be created with empty message.")]
    [InlineData("")]
    [InlineData("     ")]
    [Trait("Category", "Unit")]
    public void CantCreatResponseWithEmptyMessage(string msg)
    {
        //Arrange
        var delay = 1;

        // Act
        var exception = Record.Exception(
            () => new Notification(msg, delay, Mock.Of<IWorkSheduler>(MockBehavior.Strict), Mock.Of<ILogger<Reaction>>()));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
    }

    [Fact(DisplayName = "Notification can not be created with incorrect delay.")]
    [Trait("Category", "Unit")]
    public void CantCreatResponseWithBadDelay()
    {
        //Arrange
        var msg = "aaa";
        var delay = 0;

        // Act
        var exception = Record.Exception(
            () => new Notification(msg, delay, Mock.Of<IWorkSheduler>(MockBehavior.Strict), Mock.Of<ILogger<Reaction>>()));

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
            () => new Notification(msg, delay, Mock.Of<IWorkSheduler>(MockBehavior.Strict), Mock.Of<ILogger<Reaction>>()));

        // Assert
        exception.Should().BeNull();
    }


    [Fact(DisplayName = "Notofication can not be sended with empty socket.")]
    [Trait("Category", "Unit")]
    public async Task CantSendWithEmptyProxyAsync()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        var msg = "Test";
        var delay = 1;
        var reaction = new Notification(msg, delay, Mock.Of<IWorkSheduler>(MockBehavior.Strict), Mock.Of<ILogger<Reaction>>());
        var proxy = (IWebSocketProxy)null!;

        // Act
        var exception = await Record.ExceptionAsync(
            () => reaction.SendMessageAsync(proxy, cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "Notification could be sended.")]
    [Trait("Category", "Unit")]
    public void CouldSendNotification()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        var msg = "Test";
        var delay = 1000;
        var scheduler = new Mock<IWorkSheduler>();
        var reaction = new Notification(msg, delay, scheduler.Object, Mock.Of<ILogger<Reaction>>());

        // Act
        var t = reaction.SendMessageAsync(Mock.Of<IWebSocketProxy>(MockBehavior.Strict), cts.Token);

        // Assert
        t.IsCompleted.Should().BeTrue();
        scheduler.Verify(x => x.Schedule(It.IsAny<Func<Task>>(), cts.Token), Times.Once);
    }

    [Fact(DisplayName = "Notification cant be sended on cancelled token.")]
    [Trait("Category", "Unit")]
    public async Task CantSendNotificationOnCanceledTokenAsync()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var msg = "Test";
        var delay = 1000;
        var scheduler = new Mock<IWorkSheduler>();
        var reaction = new Notification(msg, delay, scheduler.Object, Mock.Of<ILogger<Reaction>>());

        // Act
        var exception = await Record.ExceptionAsync(
            () => reaction.SendMessageAsync(Mock.Of<IWebSocketProxy>(MockBehavior.Strict), cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<OperationCanceledException>();
    }
}
