using FluentAssertions;

using Xunit;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Loader;
using WebSocketMockServer.IO;
using WebSocketMockServer.Helpers;
using WebSocketMockServer.Reactions;

namespace WebSocketMockServer.Tests
{
    public class FileLoaderTests
    {
        [Fact(DisplayName = "FileLoader can not process null logger.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullLogger()
        {
            // Arrange
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
                () => new FileLoader(configMock.Object, null!, Mock.Of<IFileUtility>(MockBehavior.Strict), Mock.Of<IReactionFactory>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null config.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullConfig()
        {
            // Act
            var exception = Record.Exception(
                () => new FileLoader(null!, Mock.Of<ILogger<FileLoader>>(), Mock.Of<IFileUtility>(MockBehavior.Strict), Mock.Of<IReactionFactory>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null config data.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullConfigData()
        {
            // Act
            var exception = Record.Exception(
                () => new FileLoader(Mock.Of<IOptions<FileLoaderConfiguration>>(), Mock.Of<ILogger<FileLoader>>(), Mock.Of<IFileUtility>(MockBehavior.Strict), Mock.Of<IReactionFactory>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "FileLoader can not process null file utility.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullUtility()
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
                () => new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), null!, Mock.Of<IReactionFactory>()));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null reaction factory.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullLoggerFileUtility()
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
                () => new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), Mock.Of<IFileUtility>(MockBehavior.Strict), null!));

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
                () => new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), Mock.Of<IFileUtility>(MockBehavior.Strict), Mock.Of<IReactionFactory>()));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "FileLoader cant load data if token is cancelled.")]
        [Trait("Category", "Unit")]
#pragma warning disable IDE1006 // Naming Styles
        public async Task CantLoadData()
#pragma warning restore IDE1006 // Naming Styles
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

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var loggerFactory = new NullLoggerFactory();

            var loader = new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), Mock.Of<IFileUtility>(MockBehavior.Strict), Mock.Of<IReactionFactory>());

            // Act
            var exception = await Record.ExceptionAsync(
                async () => await loader.LoadAsync(cts.Token));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<OperationCanceledException>();

        }


        [Fact(DisplayName = "FileLoader can load data.")]
        [Trait("Category", "Unit")]
#pragma warning disable IDE1006 // Naming Styles
        public async Task CanLoadData()
#pragma warning restore IDE1006 // Naming Styles
        {
            //Arrange

            var folder = "A";
            var file1 = "f1";
            var file2 = "f2";
            var file3 = "f3";
            var file4 = "f4";
            var result1 = "{   \"A\": \"1\" }";
            var result2 = "{   \"A\": \"2\" }";
            var result3 = "{   \"A\": \"3\" }";
            var result4 = "{   \"A\": \"4\" }";

            var configMock = new Mock<IOptions<FileLoaderConfiguration>>(MockBehavior.Strict);
            configMock.Setup(x => x.Value).Returns(new FileLoaderConfiguration
            {
                Folder = folder,
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = file1,
                            Reactions = new[]
                            {
                                new ReactionMappingTemplate
                                {
                                     Delay = 1,
                                     File = file2
                                }
                            }
                      },
                      new RequestMappingTemplate
                      {
                            File = file3,
                            Reactions = new[]
                            {
                                new ReactionMappingTemplate
                                {
                                     File = file4
                                }
                            }
                      }
                  }
            });

            using var cts = new CancellationTokenSource();

            var fileUtilityMock = new Mock<IFileUtility>(MockBehavior.Strict);
            fileUtilityMock.Setup(x => x.ReadFileAsync(folder, file1, cts.Token)).ReturnsAsync(result1);
            fileUtilityMock.Setup(x => x.ReadFileAsync(folder, file2, cts.Token)).ReturnsAsync(result2);
            fileUtilityMock.Setup(x => x.ReadFileAsync(folder, file3, cts.Token)).ReturnsAsync(result3);
            fileUtilityMock.Setup(x => x.ReadFileAsync(folder, file4, cts.Token)).ReturnsAsync(result4);

            var loggerFactory = new NullLoggerFactory();

            var loader = new FileLoader(configMock.Object, Mock.Of<ILogger<FileLoader>>(), fileUtilityMock.Object, Mock.Of<IReactionFactory>());

            // Act
            await loader.LoadAsync(cts.Token);
            var data = loader.GetLoadedData();

            // Assert
            data.Should().NotBeNull();
            data.Should().HaveCount(2);
            data.Should().Contain(x => x.Key == StringHelpers.ReconvertWithJson(result1));
            data.Should().Contain(x => x.Key == StringHelpers.ReconvertWithJson(result3));
        }
    }
}
