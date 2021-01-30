using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using FluentAssertions;

using Xunit;

using Moq;

using WebSocketMockServer.WebSockets;

namespace WebSocketMockServer.Tests
{
    public class WebSocketProxyTests
    {
        [Fact(DisplayName = "Unable to create web socket proxy with empty socket.")]
        [Trait("Category", "Unit")]
        public void CantCreateWsProxyWithEmptySocket()
        {
            //Arrange
            WebSocket ws = null!;

            // Act
            var exception = Record.Exception(
                () => WebSocketProxy.Create(ws, null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "WebSocket proxy could be created.")]
        [Trait("Category", "Unit")]
        public void WsProxyCouldBeCreated()
        {
            //Arrange
            var ws = new Mock<WebSocket>();

            // Act
            var exception = Record.Exception(
                () => WebSocketProxy.Create(ws.Object, null!));

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
            var proxy = WebSocketProxy.Create(ws.Object, null!);

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
            var ws = new Mock<WebSocket>();
            var wresult = new ValueWebSocketReceiveResult(1, WebSocketMessageType.Binary, true);
            var vtResult = new ValueTask<ValueWebSocketReceiveResult>(Task.FromResult(wresult));
            ws.Setup(x => x.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>())).Returns(vtResult);
            var proxy = WebSocketProxy.Create(ws.Object, null!);

            // Act
            var result = await proxy.ReceiveAsync(null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            wresult.Should().Be(result);
            ws.Verify(x => x.ReceiveAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "WebSocket proxy runs SendMessageAsync properly.")]
        [Trait("Category", "Unit")]
        public void SendMessageAsyncProperly()
        {
            //Arrange
            var ws = new Mock<WebSocket>();
            ws.Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>())).Returns(new ValueTask(Task.CompletedTask));
            ws.Setup(x => x.State).Returns(WebSocketState.Open);
            var proxy = WebSocketProxy.Create(ws.Object, null!);

            // Act
            var t = proxy.SendMessageAsync("text", CancellationToken.None);

            // Assert
            t.IsCompleted.Should().BeTrue();
            ws.Verify(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);
        }

        private static TheoryData<WebSocketState> WebSocketsStatusGenerator()
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

        [Theory(DisplayName = "WebSocket proxy runs SendMessageAsync properly for not open status.")]
        [MemberData(nameof(WebSocketsStatusGenerator))]
        [Trait("Category", "Unit")]
        public void SendMessageAsyncSkipsProperly(WebSocketState testState)
        {
            //Arrange
            var ws = new Mock<WebSocket>();
            ws.Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>())).Returns(new ValueTask(Task.CompletedTask));
            ws.Setup(x => x.State).Returns(testState);
            var proxy = WebSocketProxy.Create(ws.Object, null!);

            // Act
            var t = proxy.SendMessageAsync("text", CancellationToken.None);

            // Assert
            t.IsCompleted.Should().BeTrue();
            ws.Verify(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Never);
        }

        //ADD ReceiveAsync SendMessageAsync CloseAsync with guard checks
        //ADD WebSocketsHandler tests
        //ADD WebSocketsPipelinesAdapter tests
        //ADD Integration tests
    }
}
