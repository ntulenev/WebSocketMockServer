using System;
using System.Threading;
using System.Threading.Tasks;

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
            //Arrange
            ILogger<LoaderService>? logger = null!;
            IHostApplicationLifetime lifetime = null!;
            var loader = new Mock<ILoader>();


            // Act
            var exception = Record.Exception(
                () => new LoaderService(logger, lifetime, loader.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "LoaderService can not be created with null loader.")]
        [Trait("Category", "Unit")]
        public void CantCreateLoaderServiceWithNullLoader()
        {
            //Arrange
            ILogger<LoaderService>? logger = null!;
            var lifetime = new Mock<IHostApplicationLifetime>()!;
            ILoader loader = null!;


            // Act
            var exception = Record.Exception(
                () => new LoaderService(logger, lifetime.Object, loader));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "LoaderService could be created with valid params.")]
        [Trait("Category", "Unit")]
        public void LoaderServiceCouldBeCreated()
        {
            //Arrange
            ILogger<LoaderService>? logger = null!;
            var lifetime = new Mock<IHostApplicationLifetime>()!;
            var loader = new Mock<ILoader>();


            // Act
            var exception = Record.Exception(
                () => new LoaderService(logger, lifetime.Object, loader.Object));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "LoaderService returns complete task on stop.")]
        [Trait("Category", "Unit")]
        public void LoaderServiceReturnsCompleteTaskOnStop()
        {
            //Arrange
            ILogger<LoaderService>? logger = null!;
            var lifetime = new Mock<IHostApplicationLifetime>()!;
            var loader = new Mock<ILoader>();
            var service = new LoaderService(logger, lifetime.Object, loader.Object);

            // Act
            var completeTask = service.StopAsync(CancellationToken.None);

            // Assert
            completeTask.IsCompleted.Should().BeTrue();
        }

        [Fact(DisplayName = "LoaderService loads data on start.")]
        [Trait("Category", "Unit")]
        public async Task LoaderServiceLoadsDataOnStart()
        {
            //Arrange
            ILogger<LoaderService>? logger = null!;
            var lifetime = new Mock<IHostApplicationLifetime>()!;
            var loader = new Mock<ILoader>();
            var service = new LoaderService(logger, lifetime.Object, loader.Object);

            // Act
            await service.StartAsync(CancellationToken.None);

            // Assert
            loader.Verify(x => x.LoadAsync(It.Is<CancellationToken>(a => a == CancellationToken.None)), Times.Once);
        }

        [Fact(DisplayName = "LoaderService should stop app on error.")]
        [Trait("Category", "Unit")]
        public async Task LoaderServiceShouldStopAppOnError()
        {
            //Arrange
            ILogger<LoaderService>? logger = null!;

            var lifetime = new Mock<IHostApplicationLifetime>()!;

            var loader = new Mock<ILoader>();
            loader.Setup(x => x.LoadAsync(It.IsAny<CancellationToken>())).Throws(new InvalidOperationException());

            var service = new LoaderService(logger, lifetime.Object, loader.Object);

            // Act
            await service.StartAsync(CancellationToken.None);

            // Assert
            lifetime.Verify(x => x.StopApplication(), Times.Once);
        }
    }
}
