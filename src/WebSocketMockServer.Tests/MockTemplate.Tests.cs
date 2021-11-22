using FluentAssertions;

using Xunit;

using WebSocketMockServer.Models;
using WebSocketMockServer.Storage;

namespace WebSocketMockServer.Tests
{
    public class MockTemplateTests
    {
        [Fact(DisplayName = "MockTemplate can not be created with null request.")]
        [Trait("Category", "Unit")]
        public void CantCreateMockTemplateOnNullRequest()
        {
            //Arrange
            string request = null!;

            // Act
            var exception = Record.Exception(
                () => new MockTemplate(request, null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Theory(DisplayName = "MockTemplate can not be created with empty request.")]
        [InlineData("")]
        [InlineData("  ")]
        [Trait("Category", "Unit")]
        public void CantCreateMockTemplateOnEmptyRequest(string request)
        {
            // Act
            var exception = Record.Exception(
                () => new MockTemplate(request, null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "MockTemplate can not be created with null responses.")]
        [Trait("Category", "Unit")]
        public void CantCreateMockTemplateOnNullResponses()
        {
            //Arrange
            string request = "aaa";
            IEnumerable<Reaction> resps = null!;

            // Act
            var exception = Record.Exception(
                () => new MockTemplate(request, resps));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "MockTemplate can not be created with empty responses.")]
        [Trait("Category", "Unit")]
        public void CantCreateMockTemplateOnEmptyResponses()
        {
            //Arrange
            string request = "aaa";
            IEnumerable<Reaction> resps = Enumerable.Empty<Reaction>();

            // Act
            var exception = Record.Exception(
                () => new MockTemplate(request, resps));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "MockTemplate can be created with valid data.")]
        [Trait("Category", "Unit")]
        public void MockTemplateCouldBeCreated()
        {
            //Arrange
            string request = "aaa";
            IEnumerable<Reaction> resps = new[]
            {
                Reaction.Create("A"),
                new Notification("B",1000)
            };

            // Act
            var exception = Record.Exception(
                () => new MockTemplate(request, resps));

            // Assert
            exception.Should().BeNull();
        }
    }
}
