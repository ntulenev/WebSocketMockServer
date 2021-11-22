using System.Configuration;

using FluentAssertions;

using Xunit;

using WebSocketMockServer.Configuration;

namespace WebSocketMockServer.Tests
{
    public class FileLoaderConfigurationTests
    {
        [Fact(DisplayName = "FileLoaderConfiguration could be validated.")]
        [Trait("Category", "Unit")]
        public void CanValidateFileLoaderConfigurationWithValidData()
        {
            //Arrange
            var config = new FileLoaderConfiguration
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
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "FileLoaderConfiguration cant be validated with empty folder.")]
        [Trait("Category", "Unit")]
        public void CantValidateFileLoaderConfigurationWithEmptyFolder()
        {
            //Arrange
            var config = new FileLoaderConfiguration
            {
                Folder = string.Empty,
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
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ConfigurationErrorsException>();
        }

        [Fact(DisplayName = "FileLoaderConfiguration cant be validated with null mapping.")]
        [Trait("Category", "Unit")]
        public void CantValidateFileLoaderConfigurationWithNullMapping()
        {
            //Arrange
            var config = new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = null
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ConfigurationErrorsException>();
        }

        [Theory(DisplayName = "FileLoaderConfiguration cant be validated with empty Mapping.")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [Trait("Category", "Unit")]
        public void CantValidateFileLoaderConfigurationWithEmptyMapping(string name)
        {
            //Arrange
            var config = new FileLoaderConfiguration
            {
                Folder = name,
                Mapping = Enumerable.Empty<RequestMappingTemplate>()
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ConfigurationErrorsException>();
        }

        [Theory(DisplayName = "FileLoaderConfiguration cant be validated with empty File.")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [Trait("Category", "Unit")]
        public void CantValidateFileLoaderConfigurationWithEmptyFile(string name)
        {
            //Arrange
            var config = new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = name,
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
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ConfigurationErrorsException>();
        }

        [Fact(DisplayName = "FileLoaderConfiguration cant be validated with null response.")]
        [Trait("Category", "Unit")]
        public void CantValidateFileLoaderConfigurationWithNullResponse()
        {
            //Arrange
            var config = new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = "B",
                            Reactions = null
                      }
                  }
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ConfigurationErrorsException>();
        }

        [Fact(DisplayName = "FileLoaderConfiguration cant be validated with empty response.")]
        [Trait("Category", "Unit")]
        public void CantValidateFileLoaderConfigurationWithEmptyResponse()
        {
            //Arrange
            var config = new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = "B",
                            Reactions = Enumerable.Empty<ReactionMappingTemplate>()
                      }
                  }
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ConfigurationErrorsException>();
        }


        [Theory(DisplayName = "FileLoaderConfiguration cant be validated with empty response File.")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [Trait("Category", "Unit")]
        public void CantValidateFileLoaderConfigurationWithEmptyResponseFile(string name)
        {
            //Arrange
            var config = new FileLoaderConfiguration
            {
                Folder = "A",
                Mapping = new[]
                  {
                      new RequestMappingTemplate
                      {
                            File = name,
                            Reactions = new[]
                            {
                                new ReactionMappingTemplate
                                {
                                     Delay = 1,
                                     File = string.Empty
                                }
                            }
                      }
                  }
            };

            // Act
            var exception = Record.Exception(
                () => config.Validate());

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ConfigurationErrorsException>();
        }

    }
}
