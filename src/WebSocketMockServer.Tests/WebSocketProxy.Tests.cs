using System;
using System.Net.WebSockets;

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

        //ADD  ReceiveAsync SendMessageAsync CloseAsync with guard checks
    }
}
