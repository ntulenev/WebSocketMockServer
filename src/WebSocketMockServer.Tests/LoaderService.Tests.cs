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
                () => new LoaderService(Mock.Of<ILogger<LoaderService>>(), null!, Mock.Of<ILoader>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "LoaderService can not be created with null loader.")]
        [Trait("Category", "Unit")]
        public void CantCreateLoaderServiceWithNullLoader()
        {
            // Act
            var exception = Record.Exception(
                () => new LoaderService(null!, Mock.Of<IHostApplicationLifetime>(), Mock.Of<ILoader>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "LoaderService could be created with valid params.")]
        [Trait("Category", "Unit")]
        public void LoaderServiceCouldBeCreated()
        {
            // Act
            var exception = Record.Exception(
                () => new LoaderService(Mock.Of<ILogger<LoaderService>>(), Mock.Of<IHostApplicationLifetime>(), Mock.Of<ILoader>()));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "LoaderService returns complete task on stop.")]
        [Trait("Category", "Unit")]
        public void LoaderServiceReturnsCompleteTaskOnStop()
        {
            //Arrange
            var service = new LoaderService(Mock.Of<ILogger<LoaderService>>(), Mock.Of<IHostApplicationLifetime>(), Mock.Of<ILoader>());

            // Act
            var completeTask = service.StopAsync(CancellationToken.None);

            // Assert
            completeTask.IsCompleted.Should().BeTrue();
        }

        [Fact(DisplayName = "LoaderService loads data on start.")]
        [Trait("Category", "Unit")]
        public async Task LoaderServiceLoadsDataOnStartAsync()
        {
            //Arrange
            var loader = new Mock<ILoader>();
            var service = new LoaderService(Mock.Of<ILogger<LoaderService>>(), Mock.Of<IHostApplicationLifetime>(), loader.Object);

            // Act
            await service.StartAsync(CancellationToken.None).ConfigureAwait(false);

            // Assert
            loader.Verify(x => x.LoadAsync(It.Is<CancellationToken>(a => a == CancellationToken.None)), Times.Once);
        }

        [Fact(DisplayName = "LoaderService should stop app on error.")]
        [Trait("Category", "Unit")]
        public async Task LoaderServiceShouldStopAppOnErrorAsync()
        {
            //Arrange
            var lifetime = new Mock<IHostApplicationLifetime>()!;

            var loader = new Mock<ILoader>();
            loader.Setup(x => x.LoadAsync(It.IsAny<CancellationToken>())).Throws(new InvalidOperationException());

            var service = new LoaderService(Mock.Of<ILogger<LoaderService>>(), lifetime.Object, loader.Object);

            // Act
            await service.StartAsync(CancellationToken.None).ConfigureAwait(false);

            // Assert
            lifetime.Verify(x => x.StopApplication(), Times.Once);
        }
    }
}
