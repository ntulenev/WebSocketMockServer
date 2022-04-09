using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using WebSocketMockServer.Scheduling;

using Xunit;

namespace WebSocketMockServer.Tests
{
    public class WorkShedulerTests
    {
        [Fact(DisplayName = "Unable to create WorkSheduler with null logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateWorkShedulerWithNullLogger()
        {
            // Act
            var exception = Record.Exception(
                () => new WorkSheduler(null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "WorkSheduler could be created.")]
        [Trait("Category", "Unit")]
        public void WorkShedulerCanBeCreated()
        {
            // Act
            var exception = Record.Exception(
                () => new WorkSheduler(Mock.Of<ILogger<WorkSheduler>>()));

            // Assert
            exception.Should().BeNull();
        }
    }
}
