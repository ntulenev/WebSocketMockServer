using FluentAssertions;

using Xunit;

using Moq;

using WebSocketMockServer.WebSockets;
using WebSocketMockServer.Reactions;
using Microsoft.Extensions.Logging;

namespace WebSocketMockServer.Tests
{
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
                () => new Notification(msg, delay,Mock.Of<ILogger<Reaction>>()));

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
                () => new Notification(msg, delay, Mock.Of<ILogger<Reaction>>()));

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
                () => new Notification(msg, delay, Mock.Of<ILogger<Reaction>>()));

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
                () => new Notification(msg, delay, Mock.Of<ILogger<Reaction>>()));

            // Assert
            exception.Should().BeNull();
        }


        [Fact(DisplayName = "Notofication can not be sended with empty socket.")]
        [Trait("Category", "Unit")]
        public async Task CantSendWithEmptyProxyAsync()
        {
            //Arrange
            var msg = "Test";
            var delay = 1;
            var reaction = new Notification(msg, delay, Mock.Of<ILogger<Reaction>>());
            var proxy = (IWebSocketProxy)null!;

            // Act
            var exception = await Record.ExceptionAsync(
                () => reaction.SendMessageAsync(proxy, CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "Response could be sended.")]
        [Trait("Category", "Unit")]
        public async Task CouldSendReactionAsync()
        {
            //Arrange
            var msg = "Test";
            var delay = 1000;
            var reaction = new Notification(msg, delay, Mock.Of<ILogger<Reaction>>());
            var proxy = new Mock<IWebSocketProxy>(MockBehavior.Strict);

            // Act
            var t = reaction.SendMessageAsync(proxy.Object, CancellationToken.None);

            // Assert
            t.IsCompleted.Should().BeTrue();

            proxy.Verify(x => x.SendMessageAsync(It.Is<string>(v => v == msg), It.IsAny<CancellationToken>()), Times.Never);

            await Task.Delay(delay * 2).ConfigureAwait(false); // Attempts to ensure that background task is completed.

            proxy.Verify(x => x.SendMessageAsync(It.Is<string>(v => v == msg), It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}
