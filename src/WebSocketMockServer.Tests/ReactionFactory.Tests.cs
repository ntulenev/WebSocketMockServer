using FluentAssertions;

using WebSocketMockServer.Reactions;

using Xunit;

namespace WebSocketMockServer.Tests
{
    public class ReactionFactoryTests
    {
        [Fact(DisplayName = "ReactionFactory can't be created without response delegate.")]
        [Trait("Category", "Unit")]
        public void CantBeCreatedWithoutResponseDelegate()
        {
            // Arrange
            var counter = 0;
            Notification notificationFactory(string _, int __)
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
            Notification notificationFactory(string _, int __)
            {
                notifyCounter++;
                return null!;
            }

            // Act
            var exception = Record.Exception(
                () => new ReactionFactory(responseFactory, notificationFactory));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
            respCounter.Should().Be(0);
            notifyCounter.Should().Be(0);
        }
    }
}
