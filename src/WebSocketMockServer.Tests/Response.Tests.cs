using System;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Models;
using WebSocketMockServer.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Moq;

namespace WebSocketMockServer.Tests
{
    public class ResponseTests
    {
        [Fact(DisplayName = "Response can not be created with null message.")]
        [Trait("Category", "Unit")]
        public void CantCreatResponseWithNullMessage()
        {
            //Arrange
            string msg = null!;

            // Act
            var exception = Record.Exception(
                () => Reaction.Create(msg));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Theory(DisplayName = "Response can not be created with empty message.")]
        [InlineData("")]
        [InlineData("     ")]
        [Trait("Category", "Unit")]
        public void CantCreatResponseWithEmptyMessage(string msg)
        {
            // Act
            var exception = Record.Exception(
                () => Reaction.Create(msg));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "Response can be created with valid message.")]
        [Trait("Category", "Unit")]
        public void ResponseCanBeCreated()
        {
            //Arrange
            string msg = "aaa";

            // Act
            var exception = Record.Exception(
                () => Reaction.Create(msg));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "Response can not be sended with empty socket.")]
        [Trait("Category", "Unit")]
        public async Task CantSendWithEmptyProxy()
        {
            //Arrange
            var msg = "Test";
            var reaction = Reaction.Create(msg);
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
            var reaction = Reaction.Create(msg);
            var proxy = new Mock<IWebSocketProxy>();

            // Act
            await reaction.SendMessage(proxy.Object, CancellationToken.None);

            // Assert
            proxy.Verify(x => x.SendMessageAsync(It.Is<string>(v => v == msg), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
