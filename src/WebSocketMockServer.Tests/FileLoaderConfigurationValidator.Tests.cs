using FluentAssertions;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Configuration.Validation;

using Xunit;

namespace WebSocketMockServer.Tests;

public class FileLoaderConfigurationValidatorTests
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeFalse();
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeTrue();
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeTrue();
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeTrue();
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeTrue();
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeTrue();
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeTrue();
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
        var validator = new FileLoaderConfigurationValidator();

        // Act
        var validationResult = validator.Validate(string.Empty, config);

        // Assert
        validationResult.Failed.Should().BeTrue();
    }

}
