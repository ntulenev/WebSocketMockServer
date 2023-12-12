using FluentAssertions;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using WebSocketMockServer.Reactions;
using WebSocketMockServer.Scheduling;

using Xunit;

namespace WebSocketMockServer.Tests;

public class ReactionFactoryTests
{
    [Fact(DisplayName = "ReactionFactory can't be created without response delegate.")]
    [Trait("Category", "Unit")]
    public void CantBeCreatedWithoutResponseDelegate()
    {
        // Arrange
        var counter = 0;
        Notification notificationFactory(string _, TimeSpan __)
        {
            counter++;
            return null!;
        }

        // Act
        var exception = Record.Exception(
            () => new ReactionFactory(null!, notificationFactory));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        counter.Should().Be(0);
    }

    [Fact(DisplayName = "ReactionFactory can't be created without notification delegate.")]
    [Trait("Category", "Unit")]
    public void CantBeCreatedWithoutNotificationDelegate()
    {
        // Arrange
        var counter = 0;
        Response responseFactory(string _)
        {
            counter++;
            return null!;
        }

        // Act
        var exception = Record.Exception(
            () => new ReactionFactory(responseFactory, null!));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        counter.Should().Be(0);
    }

    [Fact(DisplayName = "ReactionFactory could be created.")]
    [Trait("Category", "Unit")]
    public void CouldCreateFactory()
    {
        // Arrange
        var respCounter = 0;
        Response responseFactory(string _)
        {
            respCounter++;
            return null!;
        }

        var notifyCounter = 0;
        Notification notificationFactory(string _, TimeSpan __)
        {
            notifyCounter++;
            return null!;
        }

        // Act
        var exception = Record.Exception(
            () => new ReactionFactory(responseFactory, notificationFactory));

        // Assert
        exception.Should().BeNull();
        respCounter.Should().Be(0);
        notifyCounter.Should().Be(0);
    }

    [Fact(DisplayName = "ReactionFactory could create Response.")]
    [Trait("Category", "Unit")]
    public void CouldCreateResponse()
    {
        // Arrange
        var data = "test";
        var respCounter = 0;
        var response = new Response(data, new NullLogger<Reaction>());
        string checkData = null!;
        Response responseFactory(string _)
        {
            respCounter++;
            checkData = _;
            return response;
        }

        var notifyCounter = 0;
        Notification notificationFactory(string _, TimeSpan __)
        {
            notifyCounter++;
            return null!;
        }

        var factory = new ReactionFactory(responseFactory, notificationFactory);

        // Act
        var result = factory.Create(data);

        // Assert
        respCounter.Should().Be(1);
        checkData.Should().Be(data);
        result.Should().Be(response);
        notifyCounter.Should().Be(0);
    }

    [Fact(DisplayName = "ReactionFactory could create Notification.")]
    [Trait("Category", "Unit")]
    public void CouldCreateNotification()
    {
        // Arrange
        var data = "test";
        var delay = 1000;
        var respCounter = 0;
        var notification = new Notification(
                                data,
                                TimeSpan.FromMilliseconds(delay),
                                Mock.Of<IWorkSheduler>(MockBehavior.Strict),
                                new NullLogger<Reaction>());
        string checkData = null!;
        var checkDelay = TimeSpan.FromMilliseconds(5000);
        Response responseFactory(string _)
        {
            respCounter++;
            return null!;
        }

        var notifyCounter = 0;
        Notification notificationFactory(string _, TimeSpan __)
        {
            notifyCounter++;
            checkData = _;
            checkDelay = __;
            return notification;
        }

        var factory = new ReactionFactory(responseFactory, notificationFactory);

        // Act
        var result = factory.Create(data, TimeSpan.FromMilliseconds(delay));

        // Assert
        respCounter.Should().Be(0);
        checkData.Should().Be(data);
        checkDelay.Should().Be(TimeSpan.FromMilliseconds(delay));
        result.Should().Be(notification);
        notifyCounter.Should().Be(1);
    }
}
