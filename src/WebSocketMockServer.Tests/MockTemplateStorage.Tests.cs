using System;
using System.Collections.Generic;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Loader;
using WebSocketMockServer.Storage;

using Moq;


using WebSocketMockServer.Models;

namespace WebSocketMockServer.Tests
{
    public class MockTemplateStorageTests
    {
        [Fact(DisplayName = "MockTemplateStorage can not be created with null loader.")]
        [Trait("Category", "Unit")]
        public void CantCreateMockTemplateStorageWithNullLoader()
        {
            //Arrange
            ILoader loader = null!;

            // Act
            var exception = Record.Exception(
                () => new MockTemplateStorage(loader, null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "MockTemplateStorage creates empty storate on loader exception.")]
        [Trait("Category", "Unit")]
        public void MockTemplateCreatesEmptyStorageOnLoaderException()
        {
            //Arrange
            var loader = new Mock<ILoader>();
            loader.Setup(x => x.GetLoadedData()).Throws<InvalidOperationException>();

            // Act
            var exception = Record.Exception(
                () => new MockTemplateStorage(loader.Object, null!));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "MockTemplateStorage creates storate for correct loader.")]
        [Trait("Category", "Unit")]
        public void MockTemplateCreatesStorageOnLoader()
        {
            //Arrange
            var loader = new Mock<ILoader>();

            var testDictionary = new Dictionary<string, MockTemplate>();
            var template = new MockTemplate("aaa", new[] { Reaction.Create("bbb") });
            testDictionary.Add(template.Request, template);

            loader.Setup(x => x.GetLoadedData()).Returns(testDictionary);
            var storage = new MockTemplateStorage(loader.Object, null!);

            // Act
            var status = storage.TryGetTemplate(template.Request, out var result);

            // Assert
            status.Should().BeTrue();
            result.Should().Be(template);
        }
    }
}
