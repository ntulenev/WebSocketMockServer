using FluentAssertions;

using Xunit;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using Moq;

using WebSocketMockServer.Loader;
using WebSocketMockServer.Services;


namespace WebSocketMockServer.Tests
{
    public class LoaderServiceTests
    {
        [Fact(DisplayName = "LoaderService can not be created with null lifetime.")]
        [Trait("Category", "Unit")]
        public void CantCreateLoaderServiceWithNullLifeTime()
        {
            // Act
            var exception = Record.Exception(
                () => new LoaderService(Mock.Of<ILogger<LoaderService>>(), null!, Mock.Of<ILoader>(MockBehavior.Strict)));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "LoaderService can not be created with null loader.")]
        [Trait("Category", "Unit")]
        public void CantCreateLoaderServiceWithNullLoader()
        {
            // Act
            var exception = Record.Exception(
                () => new LoaderService(null!, Mock.Of<IHostApplicationLifetime>(MockBehavior.Strict), Mock.Of<ILoader>(MockBehavior.Strict)));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "LoaderService could be created with valid params.")]
        [Trait("Category", "Unit")]
        public void LoaderServiceCouldBeCreated()
        {
            // Act
            var exception = Record.Exception(
                () => new LoaderService(Mock.Of<ILogger<LoaderService>>(), Mock.Of<IHostApplicationLifetime>(MockBehavior.Strict), Mock.Of<ILoader>(MockBehavior.Strict)));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "LoaderService returns complete task on stop.")]
        [Trait("Category", "Unit")]
        public void LoaderServiceReturnsCompleteTaskOnStop()
        {
            //Arrange
            using var cts = new CancellationTokenSource();

            var service = new LoaderService(Mock.Of<ILogger<LoaderService>>(), Mock.Of<IHostApplicationLifetime>(MockBehavior.Strict), Mock.Of<ILoader>(MockBehavior.Strict));

            // Act
            var completeTask = service.StopAsync(cts.Token);

            // Assert
            completeTask.IsCompleted.Should().BeTrue();
        }

        [Fact(DisplayName = "LoaderService loads data on start.")]
        [Trait("Category", "Unit")]
        public async Task LoaderServiceLoadsDataOnStartAsync()
        {
            //Arrange
            using var cts = new CancellationTokenSource();

            var loader = new Mock<ILoader>();
            var service = new LoaderService(Mock.Of<ILogger<LoaderService>>(), Mock.Of<IHostApplicationLifetime>(MockBehavior.Strict), loader.Object);

            // Act
            await service.StartAsync(cts.Token).ConfigureAwait(false);

            // Assert
            loader.Verify(x => x.LoadAsync(cts.Token), Times.Once);
        }

        [Fact(DisplayName = "LoaderService stops app on fail.")]
        [Trait("Category", "Unit")]
        public async Task LoaderServiceStopsAppOnFailAsync()
        {
            //Arrange
            using var cts = new CancellationTokenSource();

            var loader = new Mock<ILoader>(MockBehavior.Strict);
            loader.Setup(x => x.LoadAsync(cts.Token)).Throws(new InvalidOperationException());

            var lifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);
            lifetime.Setup(x => x.StopApplication());

            var service = new LoaderService(Mock.Of<ILogger<LoaderService>>(), lifetime.Object, loader.Object);

            // Act
            await service.StartAsync(cts.Token).ConfigureAwait(false);

            // Assert
            lifetime.Verify(x => x.StopApplication(), Times.Once);

        }

        [Fact(DisplayName = "LoaderService skips app on cancellation.")]
        [Trait("Category", "Unit")]
        public async Task LoaderServiceDontStopsAppOnCancelAsync()
        {
            //Arrange
            using var cts = new CancellationTokenSource();

            var loader = new Mock<ILoader>(MockBehavior.Strict);
            loader.Setup(x => x.LoadAsync(cts.Token)).Throws(new TaskCanceledException());

            var lifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);
            lifetime.Setup(x => x.StopApplication());

            var service = new LoaderService(Mock.Of<ILogger<LoaderService>>(), lifetime.Object, loader.Object);

            // Act
            var exception = await Record.ExceptionAsync(async () => await service.StartAsync(cts.Token).ConfigureAwait(false));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<TaskCanceledException>();
            lifetime.Verify(x => x.StopApplication(), Times.Never);
        }
    }
}
