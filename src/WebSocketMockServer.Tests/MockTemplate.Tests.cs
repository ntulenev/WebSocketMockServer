using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Models;

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
            IEnumerable<Response> resps = null!;

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
            IEnumerable<Response> resps = Enumerable.Empty<Response>();

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
            IEnumerable<Response> resps = new[]
            {
                new Response("A"),
                new Response("B",1000)
            };

            // Act
            var exception = Record.Exception(
                () => new MockTemplate(request, resps));

            // Assert
            exception.Should().BeNull();
        }
    }
}
