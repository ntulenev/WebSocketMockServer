using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using WebSocketMockServer.Storage;
using WebSocketMockServer.WebSockets;

using Xunit;

namespace WebSocketMockServer.Tests
{
    public class WebSocketHandlerTests
    {
        [Fact(DisplayName = "WebSocketHandler cannot be created with null storage.")]
        public void CantCreateWebSocketHandlerWithNullStorage()
        {
            // Act
            var exception = Record.Exception(
                () => new WebSocketHandler(Mock.Of<ILogger<WebSocketHandler>>(), null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "WebSocketHandler cannot be created with null logger.")]
        public void CantCreateWebSocketHandlerWithNullLogger()
        {
            // Arrange
            var storage = new Mock<IMockTemplateStorage>(MockBehavior.Strict);

            // Act
            var exception = Record.Exception(
                () => new WebSocketHandler(null!, storage.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "WebSocketHandler can be created with valid params.")]
        public void CanCreateWebSocketHandler()
        {
            // Arrange
            var storage = new Mock<IMockTemplateStorage>(MockBehavior.Strict);
            var logger = new Mock<ILogger<WebSocketHandler>>();

            // Act
            var exception = Record.Exception(
                () => new WebSocketHandler(logger.Object, storage.Object));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "WebSocketHandler throws ArgumentNullException for null wsProxy.")]
        public async Task ThrowsArgumentNullExceptionForNullWsProxyAsync()
        {
            // Arrange
            var handler = new WebSocketHandler(
                Mock.Of<ILogger<WebSocketHandler>>(),
                Mock.Of<IMockTemplateStorage>(MockBehavior.Strict));
            using var cts = new CancellationTokenSource();

            // Act
            var exception = await Record.ExceptionAsync(
                    async () => await handler.HandleAsync(null!, cts.Token));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "WebSocketHandler throws OperationCanceled exception on cancelled token.")]
        public async Task ThrowsExceptionForCanceledTokenAsync()
        {
            // Arrange
            var handler = new WebSocketHandler(
                Mock.Of<ILogger<WebSocketHandler>>(),
                Mock.Of<IMockTemplateStorage>(MockBehavior.Strict));
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            var exception = await Record.ExceptionAsync(
                    async () => await handler.HandleAsync(
                                        Mock.Of<IWebSocketProxy>(MockBehavior.Strict),
                                        cts.Token));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<OperationCanceledException>();
        }
    }
}
