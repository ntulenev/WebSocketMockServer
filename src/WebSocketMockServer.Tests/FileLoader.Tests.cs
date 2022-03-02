using FluentAssertions;

using Xunit;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Loader;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

using Moq;
using WebSocketMockServer.IO;

namespace WebSocketMockServer.Tests
{
    public class FileLoaderTests
    {
        [Fact(DisplayName = "FileLoader can not process null logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullLogger()
        {
            // Arrange
            var configMock = new Mock<IOptions<FileLoaderConfiguration>>();
            configMock.Setup(x => x.Value).Returns(new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = "B",
                            Reactions = new[]
                            {
                                new ReactionMappingTemplate
                                {
                                     Delay = 1,
                                     File = "C"
                                }
                            }
                      }
                  }
            });


            // Act
            var exception = Record.Exception(
                () => new FileLoader(configMock.Object, null!, Mock.Of<ILoggerFactory>(), Mock.Of<IFileUtility>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null config.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullConfig()
        {
            // Act
            var exception = Record.Exception(
                () => new FileLoader(null!, Mock.Of<ILogger<FileLoader>>(), Mock.Of<ILoggerFactory>(), Mock.Of<IFileUtility>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null config data.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullConfigData()
        {
            //Arrange
            var config = (new Mock<IOptions<FileLoaderConfiguration>>()).Object;

            // Act
            var exception = Record.Exception(
                () => new FileLoader(config, Mock.Of<ILogger<FileLoader>>(), Mock.Of<ILoggerFactory>(), Mock.Of<IFileUtility>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "FileLoader can not process null logger factory.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullLoggerFactory()
        {
            //Arrange
            var configMock = new Mock<IOptions<FileLoaderConfiguration>>();
            configMock.Setup(x => x.Value).Returns(new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = "B",
                            Reactions = new[]
                            {
                                new ReactionMappingTemplate
                                {
                                     Delay = 1,
                                     File = "C"
                                }
                            }
                      }
                  }
            });

            // Act
            var exception = Record.Exception(
                () => new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), null!, Mock.Of<IFileUtility>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null file utility.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullLoggerFileUtility()
        {
            //Arrange
            var configMock = new Mock<IOptions<FileLoaderConfiguration>>();
            configMock.Setup(x => x.Value).Returns(new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = "B",
                            Reactions = new[]
                            {
                                new ReactionMappingTemplate
                                {
                                     Delay = 1,
                                     File = "C"
                                }
                            }
                      }
                  }
            });

            // Act
            var exception = Record.Exception(
                () => new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), Mock.Of<ILoggerFactory>(), null!));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can be created with correct data.")]
        [Trait("Category", "Unit")]
        public void CanCreateFileLoaderWithCorrectData()
        {
            //Arrange
            var configMock = new Mock<IOptions<FileLoaderConfiguration>>(MockBehavior.Strict);
            configMock.Setup(x => x.Value).Returns(new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                           File = "B",
                            Reactions = new[]
                            {
                                new ReactionMappingTemplate
                                {
                                     Delay = 1,
                                     File = "C"
                                }
                            }
                      }
                  }
            });

            // Act
            var exception = Record.Exception(
                () => new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), Mock.Of<ILoggerFactory>(), Mock.Of<IFileUtility>()));

            // Assert
            exception.Should().BeNull();
        }
    }
}
