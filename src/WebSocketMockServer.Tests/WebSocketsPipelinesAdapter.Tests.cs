using FluentAssertions;

using Moq;

using WebSocketMockServer.WebSockets;

using Xunit;

namespace WebSocketMockServer.Tests;

public class WebSocketsPipelinesAdapterTests
{
    [Fact(DisplayName = "Unable to create WebSocketsPipelinesAdapter with null socket proxy.")]
    [Trait("Category", "Unit")]
    public void CantCreateWebSocketsPipelinesAdapterWithNullSocketProxy()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        // Act
        var exception = Record.Exception(
                           () => new WebSocketsPipelinesAdapter(null!, cts.Token));
        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Theory(DisplayName = "Unable to create WebSocketsPipelinesAdapter with negative or zero buffer size.")]
    [InlineData(-1)]
    [InlineData(0)]
    [Trait("Category", "Unit")]
    public void CantCreateWebSocketsPipelinesAdapterWithNegativeOrZeroBufferSize(int bufferSize)
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        // Act
        var exception = Record.Exception(
                                        () => new WebSocketsPipelinesAdapter(
                                                  Mock.Of<IWebSocketProxy>(MockBehavior.Strict),
                                                  bufferSize,
                                                  cts.Token));
        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
    }

    [Fact(DisplayName = "WebSocketsPipelinesAdapter can be created with correct parameters.")]
    [Trait("Category", "Unit")]
    public void WebSocketsPipelinesAdapterCanBeCreated()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        // Act
        var exception = Record.Exception(
                                         () => new WebSocketsPipelinesAdapter(
                                             Mock.Of<IWebSocketProxy>(MockBehavior.Strict),
                                             1,
                                             cts.Token));
        // Assert
        exception.Should().BeNull();
    }
}
