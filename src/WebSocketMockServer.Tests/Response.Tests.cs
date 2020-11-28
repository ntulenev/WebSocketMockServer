using System;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Models;

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
                () => new Response(msg));

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
                () => new Response(msg));

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
                () => new Response(msg));

            // Assert
            exception.Should().BeNull();
        }
    }
}
