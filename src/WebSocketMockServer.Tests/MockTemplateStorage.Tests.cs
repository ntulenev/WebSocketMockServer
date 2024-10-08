using FluentAssertions;

using Xunit;

using WebSocketMockServer.Loader;
using WebSocketMockServer.Storage;
using WebSocketMockServer.Reactions;

using Moq;

using Microsoft.Extensions.Logging;

namespace WebSocketMockServer.Tests;

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
            () => new MockTemplateStorage(loader, Mock.Of<ILogger<MockTemplateStorage>>()));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "MockTemplateStorage creates empty storage on loader exception.")]
    [Trait("Category", "Unit")]
    public void MockTemplateCreatesEmptyStorageOnLoaderException()
    {
        //Arrange
        var loader = new Mock<ILoader>(MockBehavior.Strict);
        loader.Setup(x => x.GetLoadedData()).Throws<InvalidOperationException>();

        // Act
        var exception = Record.Exception(
            () => new MockTemplateStorage(loader.Object, Mock.Of<ILogger<MockTemplateStorage>>()));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "MockTemplateStorage creates storage for correct loader.")]
    [Trait("Category", "Unit")]
    public void MockTemplateCreatesStorageOnLoader()
    {
        //Arrange
        var loader = new Mock<ILoader>(MockBehavior.Strict);

        var testDictionary = new Dictionary<string, MockTemplate>();
        var template = new MockTemplate("aaa", [new Response("bbb", Mock.Of<ILogger<Reaction>>())]);
        testDictionary.Add(template.Request, template);

        loader.Setup(x => x.GetLoadedData()).Returns(testDictionary);
        var storage = new MockTemplateStorage(loader.Object, Mock.Of<ILogger<MockTemplateStorage>>());

        // Act
        var status = storage.TryGetTemplate(template.Request, out var result);

        // Assert
        status.Should().BeTrue();
        result.Should().Be(template);
    }
}
