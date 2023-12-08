using System.Net.WebSockets;

using FluentAssertions;

using Xunit;

using Moq;

using WebSocketMockServer.WebSockets;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebSocketMockServer.Tests;

public class WebSocketProxyTests
{
    public static TheoryData<WebSocketState> WebSocketsStatusGenerator()
    {
        var td = new TheoryData<WebSocketState>();
        foreach (var status in Enum.GetValues(typeof(WebSocketState)).Cast<WebSocketState>())
        {
            if (status != WebSocketState.Open)
            {
                td.Add(status);
            }
        }
        return td;
    }

    [Fact(DisplayName = "Unable to create web socket proxy with empty socket.")]
    [Trait("Category", "Unit")]
    public void CantCreateWsProxyWithEmptySocket()
    {
        //Arrange
        WebSocket ws = null!;

        // Act
        var exception = Record.Exception(
            () => WebSocketProxy.Create(ws, new NullLoggerFactory()));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "Unable to create web socket proxy with null factory.")]
    [Trait("Category", "Unit")]
    public void CantCreateWsProxyWithNullFactory()
    {
        //Arrange
        var ws = new Mock<WebSocket>(MockBehavior.Strict);

        // Act
        var exception = Record.Exception(
            () => WebSocketProxy.Create(ws.Object, null!));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "WebSocket proxy could be created.")]
    [Trait("Category", "Unit")]
    public void WsProxyCouldBeCreated()
    {
        //Arrange
        var ws = new Mock<WebSocket>(MockBehavior.Strict);

        // Act
        var exception = Record.Exception(
            () => WebSocketProxy.Create(ws.Object, new NullLoggerFactory()));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "WebSocket proxy gives correct status.")]
    [Trait("Category", "Unit")]
    public void WsProxyCouldGetStatus()
    {
        //Arrange
        var wStatus = WebSocketState.Aborted;
        var ws = new Mock<WebSocket>();
        ws.Setup(x => x.State).Returns(wStatus);
        var proxy = WebSocketProxy.Create(ws.Object, new NullLoggerFactory());

        // Act
        var status = proxy.State;

        // Assert
        wStatus.Should().Be(status);
    }

    [Fact(DisplayName = "WebSocket proxy runs ReceiveAsync properly.")]
    [Trait("Category", "Unit")]
    public async Task ReceiveAsyncShouldWorkProperlyAsync()
    {
        //Arrange
        using var cts = new CancellationTokenSource();

        var wresult = new ValueWebSocketReceiveResult(1, WebSocketMessageType.Binary, true);
        var vtResult = new ValueTask<ValueWebSocketReceiveResult>(Task.FromResult(wresult));
        var callCount = 0;

        var ws = new Mock<WebSocket>();
        ws.Setup(x => x.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
          .Returns(vtResult)
          .Callback(() => callCount++);

        var proxy = WebSocketProxy.Create(ws.Object, new NullLoggerFactory());

        // Act
        var result = await proxy.ReceiveAsync(null, cts.Token);

        // Assert
        wresult.Should().Be(result);
        callCount.Should().Be(1);
    }

    [Fact(DisplayName = "WebSocket proxy runs SendMessageAsync properly.")]
    [Trait("Category", "Unit")]
    public void SendMessageAsyncProperly()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        var callCount = 0;

        var ws = new Mock<WebSocket>(MockBehavior.Strict);
        ws.Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
          .Returns(new ValueTask(Task.CompletedTask))
          .Callback(() => callCount++);
        ws.Setup(x => x.State).Returns(WebSocketState.Open);

        var proxy = WebSocketProxy.Create(ws.Object, new NullLoggerFactory());

        // Act
        var t = proxy.SendMessageAsync("text", cts.Token);

        // Assert
        t.IsCompleted.Should().BeTrue();
        callCount.Should().Be(1);
    }

    [Theory(DisplayName = "WebSocket proxy runs SendMessageAsync properly for not open status.")]
    [MemberData(nameof(WebSocketsStatusGenerator))]
    [Trait("Category", "Unit")]
    public void SendMessageAsyncSkipsProperly(WebSocketState testState)
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        var callCount = 0;

        var ws = new Mock<WebSocket>(MockBehavior.Strict);
        ws.Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
            .Returns(new ValueTask(Task.CompletedTask))
           .Callback(() => callCount++);
        ws.Setup(x => x.State).Returns(testState);

        var proxy = WebSocketProxy.Create(ws.Object, new NullLoggerFactory());

        // Act
        var t = proxy.SendMessageAsync("text", cts.Token);

        // Assert
        t.IsCompleted.Should().BeTrue();
        callCount.Should().Be(0);
    }
}
