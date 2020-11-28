using System;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Models;

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
    }
}
