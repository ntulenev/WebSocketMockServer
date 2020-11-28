using System;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Loader;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

using Moq;

namespace WebSocketMockServer.Tests
{
    public class FileLoaderTests
    {
        [Fact(DisplayName = "FileLoader can not process null hostingEnvironment.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullEnvironment()
        {
            //Arrange
            IWebHostEnvironment hostingEnvironment = null!;
            IOptions<FileLoaderConfiguration> config = null!;
            ILogger<FileLoader>? logger = null!;


            // Act
            var exception = Record.Exception(
                () => new FileLoader(config, logger, hostingEnvironment));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null config.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullConfig()
        {
            //Arrange
            IWebHostEnvironment hostingEnvironment = (new Mock<IWebHostEnvironment>()).Object;
            IOptions<FileLoaderConfiguration> config = null!;
            ILogger<FileLoader>? logger = null!;

            // Act
            var exception = Record.Exception(
                () => new FileLoader(config, logger, hostingEnvironment));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "FileLoader can not process null config data.")]
        [Trait("Category", "Unit")]
        public void CantCreateFileLoaderWithNullConfigData()
        {
            //Arrange
            IWebHostEnvironment hostingEnvironment = (new Mock<IWebHostEnvironment>()).Object;
            IOptions<FileLoaderConfiguration> config = (new Mock<IOptions<FileLoaderConfiguration>>()).Object;
            ILogger<FileLoader>? logger = null!;

            // Act
            var exception = Record.Exception(
                () => new FileLoader(config, logger, hostingEnvironment));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "FileLoader can be created with correct data.")]
        [Trait("Category", "Unit")]
        public void CanCreateFileLoaderWithCorrectData()
        {
            //Arrange
            IWebHostEnvironment hostingEnvironment = (new Mock<IWebHostEnvironment>()).Object;
            var configMock = new Mock<IOptions<FileLoaderConfiguration>>();
            configMock.Setup(x => x.Value).Returns(new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                           File = "B",
                            Responses = new[]
                            {
                                new ResponseMappingTemplate
                                {
                                     Delay = 1,
                                     File = "C"
                                }
                            }
                      }
                  }
            });
            ILogger<FileLoader>? logger = null!;

            // Act
            var exception = Record.Exception(
                () => new FileLoader(configMock.Object, logger, hostingEnvironment));

            // Assert
            exception.Should().BeNull();
        }
    }
}
