using System;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Models;
using System.Threading.Tasks;
using WebSocketMockServer.WebSockets;
using System.Threading;
using Moq;

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
            int delay = 1;

            // Act
            var exception = Record.Exception(
                () => new Notification(msg, delay));

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
            int delay = 1;

            // Act
            var exception = Record.Exception(
                () => new Notification(msg, delay));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "Notification can not be created with incorrect delay.")]
        [Trait("Category", "Unit")]
        public void CantCreatResponseWithBadDelay()
        {
            //Arrange
            string msg = "aaa";
            int delay = 0;

            // Act
            var exception = Record.Exception(
                () => new Notification(msg, delay));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "Notification can be created with valid message.")]
        [Trait("Category", "Unit")]
        public void ResponseCanBeCreated()
        {
            //Arrange
            string msg = "aaa";
            int delay = 1;

            // Act
            var exception = Record.Exception(
                () => new Notification(msg, delay));

            // Assert
            exception.Should().BeNull();
        }


        [Fact(DisplayName = "Notofication can not be sended with empty socket.")]
        [Trait("Category", "Unit")]
        public async Task CantSendWithEmptyProxy()
        {
            //Arrange
            var msg = "Test";
            var delay = 1;
            var reaction = Reaction.Create(msg, delay);
            var proxy = (IWebSocketProxy)null!;

            // Act
            var exception = await Record.ExceptionAsync(
                () => reaction.SendMessage(proxy, CancellationToken.None));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "Response could be sended.")]
        [Trait("Category", "Unit")]
        public async Task CouldSendReaction()
        {
            //Arrange
            var msg = "Test";
            var delay = 1000;
            var reaction = Reaction.Create(msg, delay);
            var proxy = new Mock<IWebSocketProxy>();

            // Act
            var t = reaction.SendMessage(proxy.Object, CancellationToken.None);

            // Assert
            t.IsCompleted.Should().BeTrue();

            proxy.Verify(x => x.SendMessageAsync(It.Is<string>(v => v == msg), It.IsAny<CancellationToken>()), Times.Never);

            await Task.Delay(delay * 2); // Attempts to ensure that background task is completed.

            proxy.Verify(x => x.SendMessageAsync(It.Is<string>(v => v == msg), It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}
